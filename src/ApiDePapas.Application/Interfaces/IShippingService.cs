// En IShippingService.cs
using ApiDePapas.Application.DTOs;
using System.Threading.Tasks; // Usaremos async para el futuro

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingService
    {
        // El servicio tomará el request y devolverá el response ya creado
        Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest request);
    }
}