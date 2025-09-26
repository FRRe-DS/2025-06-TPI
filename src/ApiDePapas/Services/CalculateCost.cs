using ApiDePapas.Models;
using System.Collections.Generic;

namespace ApiDePapas.Services
{
    public class CalculateCost : ICalculateCost
    {
        public ShippingCostResponse CalculateShippingCost(ShippingCostRequest request)
        {
            // Lista de productos "inventados" para probar
            var productsWithCost = new List<ProductItemInput>
            {
                new ProductItemInput { Id = 1, Quantity = 2, Weight = 5, Length = 10, Width = 5, Height = 2 },
                new ProductItemInput { Id = 2, Quantity = 1, Weight = 3, Length = 5, Width = 5, Height = 5 },
                new ProductItemInput { Id = 3, Quantity = 4, Weight = 2, Length = 3, Width = 3, Height = 1 }
            };

            var response = new ShippingCostResponse
            {
                currency = "ARS",           // moneda
                total_cost = 300,           // costo fijo
                transport_type = TransportType.air,
                products = productsWithCost
            };

            return response;
        }
    }
}

