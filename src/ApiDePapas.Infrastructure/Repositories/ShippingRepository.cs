using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiDePapas.Infrastructure.Repositories
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly ApplicationDbContext _context;

        public ShippingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Implementación de IGenericRepository<ShippingDetail>
        public async Task<IEnumerable<ShippingDetail>> GetAllAsync()
        {
            // Incluye las entidades que mapeaste como Owned Entities
            return await _context.Shippings
                .Include(s => s.delivery_address_id)
                .Include(s => s.products)
                .ToListAsync();
        }

        public async Task<ShippingDetail?> GetByIdAsync(int id)
        {
            // Usamos IQueryable para construir la consulta
            var query = _context.Shippings
                
                // 1. Incluir el Travel (y sus dependencias)
                .Include(s => s.Travel) 
                    // Cargar el TransportMethod asociado al Travel
                    .ThenInclude(t => t.TransportMethod) 
                .Include(s => s.Travel)
                    // Cargar el DistributionCenter asociado al Travel
                    .ThenInclude(t => t.DistributionCenter)
                        .ThenInclude(dc => dc.Address)
                
                // 2. Incluir la Dirección de Entrega (y su Localidad)
                .Include(s => s.DeliveryAddress)
                    // Cargar la Localidad (clave compuesta) asociada a la Address
                    .ThenInclude(a => a.Locality) 

                // 3. Incluir colecciones (Owned Entities)
                .Include(s => s.products)
                .Include(s => s.logs);

            // Ejecutar la consulta
            return await query.FirstOrDefaultAsync(s => s.shipping_id == id);
        }

        public async Task AddAsync(ShippingDetail entity)
        {
            await _context.Shippings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(ShippingDetail entity)
        {
            _context.Shippings.Update(entity);
            _context.SaveChanges(); // Nota: En una app real, podrías usar un UnitOfWork
        }

        public void Delete(ShippingDetail entity)
        {
            _context.Shippings.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<ShippingDetail>> FindAsync(Expression<Func<ShippingDetail, bool>> predicate)
        {
            return await _context.Shippings.Where(predicate).ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<ShippingDetail, bool>> predicate)
        {
            return await _context.Shippings.AnyAsync(predicate);
        }

        // Implementaciones específicas de IShippingRepository
        public async Task<ShippingDetail?> GetByOrderIdAsync(int orderId)
        {
            return await GetByIdAsync(orderId); // Si el shipping_id es igual al order_id, sino ajustamos
        }
        
        public async Task<IEnumerable<ShippingDetail>> GetByUserIdAsync(int userId)
        {
            return await _context.Shippings
                .Where(s => s.user_id == userId)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int shippingId, ShippingStatus newStatus)
        {
            var shipping = await _context.Shippings.FindAsync(shippingId);
            if (shipping != null)
            {
                shipping.status = newStatus;
                _context.Shippings.Update(shipping);
                await _context.SaveChangesAsync();
            }
        }
    }
}