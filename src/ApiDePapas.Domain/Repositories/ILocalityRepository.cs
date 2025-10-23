using ApiDePapas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiDePapas.Domain.Repositories
{
    // Hereda los métodos CRUD básicos
    public interface ILocalityRepository : IGenericRepository<Locality> // Usando Locality
    {
        // Método específico para obtener por clave compuesta
        Task<Locality?> GetByCompositeKeyAsync(string postalCode, string localityName);
        Task<IEnumerable<Locality>> SearchAsync(
        string? state = null,
        string? localityName = null,
        string? postalCode = null,
        int page = 1,
        int limit = 50);
    }
}