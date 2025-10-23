using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ApiDePapas.Infrastructure.Repositories
{
    // Asumimos que también tiene la implementación de IGenericRepository<Locality>
    public class LocalityRepository : ILocalityRepository
    {
        private readonly ApplicationDbContext _context;

        public LocalityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // La implementación del método específico
        public async Task<Locality?> GetByCompositeKeyAsync(string postalCode, string localityName)
        {
            // Usamos FindAsync o FirstOrDefaultAsync para buscar por la clave compuesta.
            // EF Core maneja la clave compuesta automáticamente en la consulta.
            return await _context.Localities
                                .FirstOrDefaultAsync(l => 
                                    l.postal_code == postalCode && 
                                    l.locality_name == localityName);
        }
        public async Task<IEnumerable<Locality>> SearchAsync(
            string? state = null,
            string? localityName = null,
            string? postalCode = null,
            int page = 1,
            int limit = 50)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 50;

            IQueryable<Locality> q = _context.Localities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(state))
                q = q.Where(l => l.state_name.ToUpper() == state.ToUpper());            // si tu prop se llama 'province', cambiala aquí

            if (!string.IsNullOrWhiteSpace(localityName))
                q = q.Where(l => l.locality_name.ToUpper() == localityName.ToUpper());

            if (!string.IsNullOrWhiteSpace(postalCode))
                q = q.Where(l => l.postal_code == postalCode);

            q = q.OrderBy(l => l.state_name)
                .ThenBy(l => l.locality_name)
                .ThenBy(l => l.postal_code);

            return await q.Skip((page - 1) * limit)
                        .Take(limit)
                        .ToListAsync();
        }
        // --- Implementaciones de IGenericRepository<Locality> van aquí ---
        public Task<IEnumerable<Locality>> GetAllAsync() => throw new NotImplementedException();
        public Task<Locality?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task AddAsync(Locality entity) => throw new NotImplementedException();
        public void Update(Locality entity) => throw new NotImplementedException();
        public void Delete(Locality entity) => throw new NotImplementedException();
        public Task<IEnumerable<Locality>> FindAsync(Expression<Func<Locality, bool>> predicate) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Expression<Func<Locality, bool>> predicate) => throw new NotImplementedException();
    }
}