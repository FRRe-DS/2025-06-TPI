using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;

namespace ApiDePapas.Application.Services
{
    public class ShippingService : IShippingService
    {
        private readonly ICalculateCost _calculate_cost;
        private readonly IShippingRepository _shipping_repository;
        private readonly ILocalityRepository _locality_repository;
        private readonly IAddressRepository _address_repository;
        private readonly ITravelRepository _travel_repository;
        private readonly IMessagePublisher _publisher;

        public ShippingService(
            ICalculateCost calculateCost,
            IShippingRepository shippingRepository,
            ILocalityRepository localityRepository,
            IAddressRepository addressRepository,
            ITravelRepository travelRepository,
            IMessagePublisher publisher)
        {
            _calculate_cost = calculateCost;
            _shipping_repository = shippingRepository;
            _locality_repository = localityRepository;
            _address_repository = addressRepository;
            _travel_repository = travelRepository;
            _publisher = publisher;
        }

        public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            if (req == null || req.products == null || req.products.Count == 0)
                return null;

            var costReq = new ShippingCostRequest(
                req.delivery_address,
                req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
            );
            var cost = _calculate_cost.CalculateShippingCost(costReq);
            int default_estimated_days = 3;

            var locality = await _locality_repository.GetByCompositeKeyAsync(
                req.delivery_address.postal_code,
                req.delivery_address.locality_name);

            if (locality == null) return null;

            var existingAddress = await _address_repository.FindExistingAddressAsync(
                req.delivery_address.street,
                req.delivery_address.number,
                req.delivery_address.postal_code,
                req.delivery_address.locality_name);

            if (existingAddress == null)
            {
                var newAddress = new Address
                {
                    street = req.delivery_address.street,
                    number = req.delivery_address.number,
                    postal_code = req.delivery_address.postal_code,
                    locality_name = req.delivery_address.locality_name
                };
                await _address_repository.AddAsync(newAddress);
                existingAddress = newAddress;
            }

            int delivery_address_id = existingAddress.address_id;

            int travel_id = await _travel_repository.AssignToExistingOrCreateNewTravelAsync(
                distributionCenterId: 1,
                transportMethodId: 1
            );

            var newShipping = new ShippingDetail
            {
                order_id = req.order_id,
                user_id = req.user_id,
                travel_id = travel_id,
                delivery_address_id = delivery_address_id,
                products = req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList(),
                status = ShippingStatus.created,
                total_cost = (float)cost.total_cost,
                currency = cost.currency,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                estimated_delivery_at = DateTime.UtcNow.AddDays(default_estimated_days),
                tracking_number = Guid.NewGuid().ToString(),
                carrier_name = "PENDIENTE",
                logs = new List<ShippingLog>
                {
                    new ShippingLog(DateTime.UtcNow, ShippingStatus.created, "Shipping created in DB.")
                }
            };

            await _shipping_repository.AddAsync(newShipping);

            var response = new CreateShippingResponse(
                shipping_id: newShipping.shipping_id,
                status: newShipping.status,
                transport_type: req.transport_type,
                estimated_delivery_at: newShipping.estimated_delivery_at
            );

            await _publisher.PublishAsync(new
            {
                Event = "ShippingCreated",
                ShippingId = response.shipping_id,
                Status = response.status.ToString(),
                CreatedAt = DateTime.UtcNow
            }, "logistics.exchange");

            return response;
        }
    }
}
