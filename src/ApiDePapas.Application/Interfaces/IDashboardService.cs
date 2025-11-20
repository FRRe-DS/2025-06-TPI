using System.Collections.Generic;
using System.Threading.Tasks;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<DashboardShipmentDto>> GetDashboardShipmentsAsync(int page, int pageSize);
        Task<int> GetTotalDashboardShipmentsCountAsync();
        Task UpdateShipmentStatusAsync(int id, ShippingStatus newStatus);
    }
}
