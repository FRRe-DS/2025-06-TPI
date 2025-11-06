using System;
using System.Linq;
using ApiDePapas.Application.DTOs;
using LogisticsApi.Application.DTOs; //LUEGO CORREGIR ESTE ERROR DE NAMESPACE
using ApiDePapas.Application.Interfaces;
using System.Threading.Tasks;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using System.Collections.Generic;
//Este using es sospechoso !!!!!!!!!!!!!!!!!!!!!
using Microsoft.EntityFrameworkCore;

namespace ApiDePapas.Application.Services
{
    public class ShippingService : IShippingService
    {
        // Se mantienen todas las inyecciones de dependencia
        private readonly ICalculateCost _calculate_cost;
        private readonly IShippingRepository _shipping_repository;
        private readonly ILocalityRepository _locality_repository;
        private readonly IAddressRepository _address_repository;
        private readonly ITravelRepository _travel_repository;

        // Constructor se mantiene
        public ShippingService(
            ICalculateCost calculateCost,
            IShippingRepository shippingRepository,
            ILocalityRepository localityRepository,
            IAddressRepository addressRepository,
            ITravelRepository travelRepository)
        {
            _calculate_cost = calculateCost;
            _shipping_repository = shippingRepository;
            _locality_repository = localityRepository;
            _address_repository = addressRepository;
            _travel_repository = travelRepository;
        }

        // IMPLEMENTACIÓN DE CREACIÓN (ACTUALIZADA: Manejo de errores con null)
        public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            // CAMBIO: Unificación de validación y retorno de null en lugar de lanzar ArgumentException
            if (req == null || req.products == null || req.products.Count == 0)
                return null;

            var costReq = new ShippingCostRequest(
                req.delivery_address,
                req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
            );
            var cost = _calculate_cost.CalculateShippingCost(costReq);

            int default_estimated_days = 3;

            var locality = await _locality_repository.GetByCompositeKeyAsync(
                req.delivery_address.postal_code,
                req.delivery_address.locality_name);

            if (locality == null) return null;

            // ... (Resto de la lógica de creación de Address, Travel, y mapeo a ShippingDetail es igual a tu código ACTUAL)
            // ...

            var existingAddress = await _address_repository.FindExistingAddressAsync(
                req.delivery_address.street,
                req.delivery_address.number,
                req.delivery_address.postal_code,
                req.delivery_address.locality_name);

            if (existingAddress == null)
            {
                var newAddress = new Address
                {
                    street = req.delivery_address.street,
                    number = req.delivery_address.number,
                    postal_code = req.delivery_address.postal_code,
                    locality_name = req.delivery_address.locality_name
                };
                await _address_repository.AddAsync(newAddress);
                existingAddress = newAddress;
            }
            int delivery_address_id = existingAddress.address_id;

            int travel_id = await _travel_repository.AssignToExistingOrCreateNewTravelAsync(
                distributionCenterId: 1,
                transportMethodId: 1
            );

            var newShipping = new ShippingDetail
            {
                order_id = req.order_id,
                user_id = req.user_id,

                travel_id = travel_id,
                delivery_address_id = delivery_address_id,

                products = req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList(),

                status = ShippingStatus.created,
                total_cost = (float)cost.total_cost,
                currency = cost.currency,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,

                estimated_delivery_at = DateTime.UtcNow.AddDays(default_estimated_days),

                tracking_number = Guid.NewGuid().ToString(),
                carrier_name = "PENDIENTE",
                logs = new List<ShippingLog>(new[] { new ShippingLog(DateTime.UtcNow, ShippingStatus.created, "Shipping created in DB.") })
            };

            await _shipping_repository.AddAsync(newShipping);

            return new CreateShippingResponse(
                shipping_id: newShipping.shipping_id,
                status: newShipping.status,
                transport_type: req.transport_type,
                estimated_delivery_at: newShipping.estimated_delivery_at
            );
        }

        // IMPLEMENTACIÓN REINTRODUCIDA (De la rama VIEJA)
        public async Task<ShippingDetail?> GetAsync(int id)
        => await _shipping_repository.GetByIdAsync(id);

        // IMPLEMENTACIÓN REINTRODUCIDA (De la rama VIEJA)
        public async Task<CancelShippingResponse> CancelAsync(int id, DateTime whenUtc)
        {
            var s = await _shipping_repository.GetByIdAsync(id);
            if (s is null)
                throw new KeyNotFoundException($"Shipping {id} not found");

            if (s.status is ShippingStatus.delivered or ShippingStatus.cancelled)
                throw new InvalidOperationException(
                    $"Shipping {id} cannot be cancelled in state '{s.status}'.");

            await _shipping_repository.UpdateStatusAsync(id, ShippingStatus.cancelled);

            return new CancelShippingResponse(
                shipping_id: id,
                status: ShippingStatus.cancelled,
                cancelled_at: whenUtc
            );
        }

        // IMPLEMENTACIÓN REINTRODUCIDA (De la rama VIEJA)
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
                                : TransportType.truck,
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
}