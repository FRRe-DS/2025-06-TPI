using ApiDePapas.Models;

namespace ApiDePapas.Services
{
    public interface IShippingService
    {
        ShippingCostResponse CalculateShippingCost(ShippingCostRequest request);
    }
}
