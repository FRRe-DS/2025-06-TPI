using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingStore
    {
        IEnumerable<ShippingDetail> GetAll();
        // (Más métodos como Create/GetById/Cancel pueden existir en otras ramas)
    }
}
 