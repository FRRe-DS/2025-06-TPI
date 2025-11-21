using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using ApiDePapas.Infrastructure.Persistence;
using ApiDePapas.Domain.Queries;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;

namespace ApiDePapas.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> FindExistingAddressAsync(AddressQuery query)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => 
                a.street == query.Street && 
                a.number == query.Number &&
                a.postal_code == query.PostalCode &&
                a.locality_name == query.LocalityName);
    }

    // Implementación de IGenericRepository<Address>
    public async Task AddAsync(Address entity)
    {
        await _context.Addresses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    // Nota: Debes implementar los demás métodos CRUD de IGenericRepository aquí (GetAllAsync, GetByIdAsync, etc.)
    public Task<IEnumerable<Address>> GetAllAsync() => throw new NotImplementedException();
    public Task<Address?> GetByIdAsync(int id) => throw new NotImplementedException();
    public void Update(Address entity) => throw new NotImplementedException();
    public void Delete(Address entity) => throw new NotImplementedException();
    public Task<IEnumerable<Address>> FindAsync(Expression<Func<Address, bool>> predicate) => throw new NotImplementedException();
    public Task<bool> ExistsAsync(Expression<Func<Address, bool>> predicate) => throw new NotImplementedException();
}