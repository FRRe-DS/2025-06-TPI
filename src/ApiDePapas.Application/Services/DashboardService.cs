using Microsoft.EntityFrameworkCore;

using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Repositories;

namespace ApiDePapas.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IShippingRepository _shippingRepository;

    public DashboardService(IShippingRepository shippingRepository)
    {
        _shippingRepository = shippingRepository;
    }

    public async Task<IEnumerable<DashboardShipmentDto>> GetDashboardShipmentsAsync(
        int page, int pageSize, string? id, string? city, string? status, DateTime? startDate, DateTime? endDate)
    {
        IQueryable<ApiDePapas.Domain.Entities.ShippingDetail> query = _shippingRepository.GetAllQueryable()
            .Include(s => s.DeliveryAddress)
                .ThenInclude(da => da.Locality)
            .Include(s => s.Travel)
                .ThenInclude(t => t.TransportMethod)
            .Include(s => s.Travel)
                .ThenInclude(t => t.DistributionCenter)
                    .ThenInclude(dc => dc.Address);

        // Apply filters
        if (!string.IsNullOrEmpty(id))
        {
            query = query.Where(s => s.shipping_id.ToString().Contains(id));
        }
        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(s => s.DeliveryAddress.locality_name.Contains(city));
        }
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
        }
        if (startDate.HasValue)
        {
            query = query.Where(s => s.created_at >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(s => s.created_at <= endDate.Value);
        }

        var shipments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return shipments.Select(s => new DashboardShipmentDto
        {
            shipping_id = s.shipping_id,
            order_id = s.order_id,
            user_id = s.user_id,
            products = s.products,
            status = s.status,
            transport_type = s.Travel.TransportMethod.transport_type,
            estimated_delivery_at = s.estimated_delivery_at,
            created_at = s.created_at,
            delivery_address = new AddressReadDto
            {
                address_id = s.DeliveryAddress.address_id,
                street = s.DeliveryAddress.street,
                number = s.DeliveryAddress.number,
                postal_code = s.DeliveryAddress.postal_code,
                locality_name = s.DeliveryAddress.locality_name,
            },
            departure_address = new AddressReadDto
            {
                address_id = s.Travel.DistributionCenter.Address.address_id,
                street = s.Travel.DistributionCenter.Address.street,
                number = s.Travel.DistributionCenter.Address.number,
                postal_code = s.Travel.DistributionCenter.Address.postal_code,
                locality_name = s.Travel.DistributionCenter.Address.locality_name,
            }
        });
    }

    public async Task<int> GetTotalDashboardShipmentsCountAsync(
        string? id, string? city, string? status, DateTime? startDate, DateTime? endDate)
    {
        var query = _shippingRepository.GetAllQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(id))
        {
            query = query.Where(s => s.shipping_id.ToString().Contains(id));
        }
        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(s => s.DeliveryAddress.locality_name.Contains(city));
        }
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
        }
        if (startDate.HasValue)
        {
            query = query.Where(s => s.created_at >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(s => s.created_at <= endDate.Value);
        }

        return await query.CountAsync();
    }
}