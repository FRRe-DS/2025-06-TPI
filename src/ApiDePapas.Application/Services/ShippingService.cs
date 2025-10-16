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
        private readonly IShippingStore _store;
        private readonly ICalculateCost _calculateCost;

        public ShippingService(IShippingStore store, ICalculateCost calculateCost)
        {
            _store = store;
            _calculateCost = calculateCost;
        }

        public Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            // 1. Validación de negocio
            if (req.products == null || req.products.Count == 0)
            {
                // Si la validación falla, simplemente devolvemos null
                return Task.FromResult<CreateShippingResponse?>(null);

            }

            // 2. El resto de la lógica sigue igual

            var ProductQuantities = req.products.Select(p => new ProductQty
            (
                p.id,
                p.quantity
            )).ToList();

            var costReq = new ShippingCostRequest(req.delivery_address, ProductQuantities);
            _calculateCost.CalculateShippingCost(costReq);

            // acá iriá el método que solicitamos a BDvar id = asnda

            var created = new CreateShippingResponse(
                0,
                ShippingStatus.created,
                req.transport_type,
                DateTime.UtcNow.AddDays(3)
            );


            //created debería ser un método para pedir a infraestructura el id que necesitamos

            //var id = _store.Save(created);
            //created.shipping_id = id;

            return Task.FromResult<CreateShippingResponse?>(created);
        }
    }
}