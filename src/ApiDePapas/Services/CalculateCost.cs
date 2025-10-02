using ApiDePapas.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Services
{
    public class CalculateCost : ICalculateCost
    {
        private readonly IStockService _stockService;

        public CalculateCost(IStockService stockService)
        {
            _stockService = stockService;
        }


        public ShippingCostResponse CalculateShippingCost(ShippingCostRequest request)
        {
            var details = _stockService.GetProductsDetail(request.products);

            float totalCost = 0;
            var productsWithCost = new List<ProductOutput>();

            foreach (var d in details)
            {
                float varcost = d.width + d.weight + 20; // ejemplo simple
                total_cost += varcost;
                productsWithCost.Add(new ProductOutput
                {
                    id = d.id,
                    cost = varcost
                });
            }

            var response = new ShippingCostResponse
            {
                currency = "ARS",           // moneda
                total_cost = totalCost,     // costo fijo
                transport_type = TransportType.air,
                products = productsWithCost
            };

            return response;
        }
    }
}
