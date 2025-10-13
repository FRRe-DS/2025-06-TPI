using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Application.Interfaces
{
    public interface ICalculateCost
    {
        ShippingCostResponse CalculateShippingCost(ShippingCostRequest request);
    }
}
