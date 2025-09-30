using ApiDePapas.Models;
using System.Collections.Generic;

namespace ApiDePapas.Services
{
    public class CalculateCost : ICalculateCost
    {
        public ShippingCostResponse CalculateShippingCost(ShippingCostRequest request)
        {
            float totalCost = 0;
            var productsWithCost = new List<ProductItemOutput>();

            foreach (var p in request.products)
            {
                float cost = p.Weight * p.Quantity * 10; // ejemplo simple
                totalCost += cost;
                productsWithCost.Add(new ProductItemOutput
                {
                    Id = p.Id,
                    Cost = cost
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

