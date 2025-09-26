using ApiDePapas.Models;

namespace ApiDePapas.Services
{
    public interface ICalculateCost
    {
        ShippingCostResponse CalculateShippingCost(ShippingCostRequest request);
    }
}
