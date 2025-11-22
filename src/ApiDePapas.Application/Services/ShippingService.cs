using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Extensions;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Domain.ValueObjects;

// Preferiblemente habría que refactorizar para que
// el EFC no sea necesario aca y delegar esas responsabilidades
// a alguna clase en la capa infrastructure
using Microsoft.EntityFrameworkCore;

namespace ApiDePapas.Application.Services;

public class ShippingService : IShippingService
{
    // Se mantienen todas las inyecciones de dependencia
    private readonly ICalculateCost _calculate_cost;
    private readonly IShippingRepository _shipping_repository;
    private readonly ILocalityRepository _locality_repository;
    private readonly IAddressRepository _address_repository;
    private readonly ITravelRepository _travel_repository;
    private readonly IPurchasingService _purchasing_service;

    public ShippingService(
        ICalculateCost calculateCost,
        IShippingRepository shippingRepository,
        ILocalityRepository localityRepository,
        IAddressRepository addressRepository,
        ITravelRepository travelRepository,
        IPurchasingService purchasingService)
    {
        _calculate_cost = calculateCost;
        _shipping_repository = shippingRepository;
        _locality_repository = localityRepository;
        _address_repository = addressRepository;
        _travel_repository = travelRepository;
        _purchasing_service = purchasingService;
    }

    public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
    {
        if (req == null || !req.IsValid())
            return null;

        var costReq = new ShippingCostRequest(req);
        var cost = await _calculate_cost.CalculateShippingCostAsync(costReq);

        int default_estimated_days = 3;

        var locality = await _locality_repository.GetByCompositeKeyAsync(
            req.delivery_address.postal_code,
            req.delivery_address.locality_name);

        if (locality == null) return null;

        var existingAddress = await _address_repository.FindExistingAddressAsync(req.delivery_address.ToQuery());

        if (existingAddress == null)
        {
            var newAddress = req.delivery_address.ToAddress();
            await _address_repository.AddAsync(newAddress);
            existingAddress = newAddress;
        }

        int delivery_address_id = existingAddress.address_id;

        int travel_id = await _travel_repository.AssignToExistingOrCreateNewTravelAsync(
            distributionCenterId: 1,
            transportMethodId: 1
        );

        var newShipping = new ShippingDetail(
            req.order_id, req.user_id, travel_id, delivery_address_id,
            (float)cost.total_cost, cost.currency, req.products.ToProductQtyList(), default_estimated_days
        );

        await _shipping_repository.AddAsync(newShipping);

        return new CreateShippingResponse(
            shipping_id: newShipping.shipping_id,
            status: newShipping.status,
            transport_type: req.transport_type,
            estimated_delivery_at: newShipping.estimated_delivery_at
        );
    }

    public async Task<ShippingDetailResponse?> GetByIdAsync(int id)
    {
        var data = await _shipping_repository.GetByIdAsync(id);
        if (data is null)
        {
            return null; 
        }
        
        return new ShippingDetailResponse(data);
    }

    public async Task<CancelShippingResponse> CancelAsync(int id, DateTime whenUtc)
    {
        var s = await _shipping_repository.GetByIdAsync(id);
        if (s is null)
            throw new KeyNotFoundException($"Shipping {id} not found");

        if (s.IsCancellable())
            throw new InvalidOperationException(
                $"Shipping {id} cannot be cancelled in state '{s.status}'.");

        await _shipping_repository.UpdateStatusAsync(id, ShippingStatus.Canceled);

        // Notify the purchasing service about the cancellation.
        // We don't want to block the response while waiting for this, so we don't await the task.
        // A more robust solution might involve a background job or a message queue.
        _ = _purchasing_service.NotifyShippingCancellationAsync(id);

        return new CancelShippingResponse(
            shipping_id: id,
            status: ShippingStatus.Canceled,
            cancelled_at: whenUtc
        );
    }

    public async Task<ShippingListResponse> List(
        int? userId, ShippingStatus? status, DateOnly? fromDate, DateOnly? toDate, int page, int limit)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 20;

        // Utilizamos GetAllQueryable() de ShippingRepository (versión ACTUAL)
        var query = _shipping_repository.GetAllQueryable();

        // Aplicamos los filtros
        if (userId.HasValue) query = query.Where(s => s.user_id == userId.Value);
        if (status.HasValue) query = query.Where(s => s.status == status.Value);
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(s => s.created_at >= from);
        }
        if (toDate.HasValue)
        {
            var to = toDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(s => s.created_at <= to);
        }

        // Orden y paginado
        var ordered = query.OrderByDescending(s => s.created_at);
        var total = await ordered.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);
        var slice = await ordered.Skip((page - 1) * limit).Take(limit).ToListAsync();

        var summaries = slice.Select(s => new ShipmentSummary(
            ShippingId: s.shipping_id,
            OrderId: s.order_id,
            UserId: s.user_id,
            Products: s.products?.ToList() ?? new List<ProductQty>(),
            Status: s.status,
            TransportType: (s.Travel != null && s.Travel.TransportMethod != null)
                            ? s.Travel.TransportMethod.transport_type
                            : TransportType.Truck,
            EstimatedDeliveryAt: s.estimated_delivery_at,
            CreatedAt: s.created_at
        )).ToList();

        var pagination = new PaginationData(
            current_page: page,
            total_pages: totalPages,
            total_items: total,
            items_per_page: limit
        );

        return new ShippingListResponse(summaries, pagination);
    }
}