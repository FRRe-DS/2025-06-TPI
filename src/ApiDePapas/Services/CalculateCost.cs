using ApiDePapas.Models;
using System.Linq;

namespace ApiDePapas.Services
{
    public class CalculateCost : ICalculateCost
    {
        public ShippingCostResponse CalculateShippingCost(ShippingCostRequest request)
        {
            // Ejemplo básico: costo = suma de (peso * 10) por cada producto
            var productCosts = request.Products.Select(p =>
                new ShippingProductCost
                {
                    Id = p.Id,
                    Cost = p.Weight * p.Quantity * 10 // regla simple
                }).ToList();

            var totalCost = productCosts.Sum(p => p.Cost);

            // Determinar tipo de transporte simple (ejemplo)
            string transportType = totalCost > 50 ? "air" : "land";

            return new ShippingCostResponse
            {
                Currency = "ARS",
                TotalCost = totalCost,
                TransportType = transportType,
                Products = productCosts
            };
        }
    }
}