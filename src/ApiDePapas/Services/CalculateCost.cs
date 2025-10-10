using ApiDePapas.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/* 
 * Quotes cost for a shipment without creating any resources.
 * Used by Order Management module to show shipping options to customers before purchase.
 *
 * Integration Flow:
 * 1. Order Management sends only: delivery_address + product IDs with quantities
 * 2. Logistics queries Stock module for EACH product:
 *    - GET /products/{id} → returns weight, dimensions, warehouse_postal_code
 * 3. Logistics calculates:
 *    - Total weight = sum(product.weight * quantity)
 *    - Total volume = sum(product dimensions * quantity)
 *    - Distance = from warehouse_postal_code to delivery_address.postal_code
 * 4. Returns estimated cost based on weight, volume, distance, and transport type
 * 5. NO data is persisted (quote only)
 */

/*
 * Se tiene que tener en cuenta el medio de transporte para calcular el precio?
 * Como se determina qué medio de transporse se utiliza?
 */

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
            float total_cost = 0;
            List<ProductOutput> products_with_cost = new List<ProductOutput>();

            foreach (ProductQty prod in request.products)
            {
                var prod_detail = _stockService.GetProductDetail(prod);

                float total_weight_grs = prod_detail.weight * prod.quantity;

                float prod_volume_cm3 = prod_detail.length * prod_detail.width * prod_detail.height;
                float total_volume = prod_volume_cm3 * prod.quantity;

                // Hay que calcular utilizando una api o algo segun los codigos postales.
                // float distance_km = _CalculateDistance(request.delivery_address.postal_code, prod_detail.postal_code);
                float distance_km = 500;

                // Calcular costo, formula de ejemplo
                float partial_cost = total_weight_grs * 1.2f + prod_volume_cm3 * 0.5f + distance_km * 8.0f;

                total_cost += partial_cost;

                products_with_cost.Add(new ProductOutput(prod.id, partial_cost));
            }

            var response = new ShippingCostResponse("ARS", total_cost, TransportType.air, products_with_cost);

            return response;
        }
    }
}
