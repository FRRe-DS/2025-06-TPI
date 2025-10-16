/*using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain;

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingStore
    {
        int Save(CreateShippingResponse response);
        CreateShippingResponse? Get(int id);
    }
}
*/
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingStore
    {
        int Save(CreateShippingResponse response);
        CreateShippingResponse? GetById(int shippingId);
        // Otros métodos necesarios para manejar envíos
        // Por ejemplo, actualizar estado, listar envíos, etc.
        //esto permite leer el envío por su ID
    }
}