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
    }
}