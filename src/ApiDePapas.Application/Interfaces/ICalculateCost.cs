using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Application.Interfaces;

public interface ICalculateCost
{
    Task<ShippingCostResponse> CalculateShippingCostAsync(ShippingCostRequest request);
}
