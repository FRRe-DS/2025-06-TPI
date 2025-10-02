using ApiDePapas.Models;
using System.Collections.Generic;

namespace ApiDePapas.Services
{
    public class CalculateCost : ICalculateCost
    {
        public ShippingCostResponse CalculateShippingCost(ShippingCostRequest request)
        {
            float totalCost = 0;
            var productsWithCost = new List<ProductOutput>();

            foreach (var p in request.products)
            {
                float varcost = p.cost + 20; // ejemplo simple
                totalCost += varcost;
                productsWithCost.Add(new ProductOutput
                {
                    id = p.id,
                    cost = varcost
                });
            }
            var response = new ShippingCostResponse
            {
                currency = "ARS",           // moneda
                total_cost = totalCost,           // costo fijo
                transport_type = TransportType.air,
                products = productsWithCost
            };

            return response;
        }
    }
}

