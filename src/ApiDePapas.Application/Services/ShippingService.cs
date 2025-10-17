using System;
using System.Linq;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using System.Threading.Tasks;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Services
{
    public class ShippingService : IShippingService
    {
        private readonly ICalculateCost _calculateCost;
        private readonly IShippingStore _store;

        public ShippingService(ICalculateCost calculateCost, IShippingStore store)
        {
            _calculateCost = calculateCost;
            _store = store;
        }

        public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            // Validación mínima: mismo criterio que venías usando
            if (req == null || req.products == null || req.products.Count == 0)
                return null; // El controller responde 422 (tu patrón)

            // Reutilizamos EXACTO el flujo de /shipping/cost
            var costReq = new ShippingCostRequest(
                req.delivery_address,
                // OJO: ProductRequest tiene 'id' y 'quantity' (no 'product_id')
                req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
            );

            var cost = _calculateCost.CalculateShippingCost(costReq); // misma fuente de verdad del cálculo
            // (ICalculateCost ya lo usás en ShippingCostController) :contentReference[oaicite:1]{index=1} 

            // Armamos la respuesta de creación (simple y directa)
            var created = new CreateShippingResponse(
                shipping_id: 0, // lo sobreescribimos con el ID del store
                status: ShippingStatus.created,
                transport_type: req.transport_type ?? TransportType.road,
                estimated_delivery_at: DateTime.UtcNow.AddDays(3) // placeholder simple
            );

            // Guardamos usando TU store actual y obtenemos ID real
            var newId = _store.Save(created);
            var final = new CreateShippingResponse(
                shipping_id: newId,
                status: created.status,
                transport_type: created.transport_type,
                estimated_delivery_at: created.estimated_delivery_at
            );

            // simulamos async para respetar la firma del contrato
            await Task.CompletedTask;
            return final;
        }
    }
}
