using ApiDePapas.Domain.Entities;
using System.Linq;

namespace ApiDePapas.Domain.Repositories
{
    // El repositorio de Shipping hereda las operaciones CRUD básicas
    public interface IShippingRepository : IGenericRepository<ShippingDetail>
    {
        // Operaciones específicas para ShippingDetail
        Task<ShippingDetail?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<ShippingDetail>> GetByUserIdAsync(int userId);
        
                // Operación para el patrón CQRS/actualización atómica
        
                Task UpdateStatusAsync(int shippingId, ShippingStatus newStatus);
        
        
        
                IQueryable<ShippingDetail> GetAllQueryable(); // Used for pagination with complex includes
        
        
        
                IQueryable<ShippingDetail> GetQueryableForStatistics(); // New clean method for statistics
        
            }
        
        }
        
        