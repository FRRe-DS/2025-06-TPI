using System;
using System.Linq;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using System.Threading.Tasks;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using System.Collections.Generic;

namespace ApiDePapas.Application.Services
{
    public class ShippingService : IShippingService
    {
        private readonly ICalculateCost _calculate_cost;
        private readonly IShippingRepository _shipping_repository;
        private readonly ILocalityRepository _locality_repository;
        private readonly IAddressRepository _address_repository;
        private readonly ITravelRepository _travel_repository;
        
        // Constructor, inyecciones...
        public ShippingService(
            ICalculateCost calculateCost, 
            IShippingRepository shippingRepository,
            ILocalityRepository localityRepository,
            IAddressRepository addressRepository,
            ITravelRepository travelRepository)
        {
            _calculate_cost = calculateCost;
            _shipping_repository = shippingRepository;
            _locality_repository = localityRepository;
            _address_repository = addressRepository;
            _travel_repository = travelRepository;
        }

        public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            // 1. Validación Mínima y Cálculo de Costo
            if (req == null || req.products == null || req.products.Count == 0)
                return null;

            var costReq = new ShippingCostRequest(
                req.delivery_address,
                req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
            );
            var cost = _calculate_cost.CalculateShippingCost(costReq);

            // **IMPORTANTE: Si 'cost' NO contiene los días, usaremos un valor por defecto.**
            // Asumo que quieres usar 3 días, como en tu código original.
            int default_estimated_days = 3; 

            // 2. LÓGICA DE NORMALIZACIÓN Y FKs
            
            // A. VALIDAR LOCALITY
            var locality = await _locality_repository.GetByCompositeKeyAsync(
                req.delivery_address.postal_code, 
                req.delivery_address.locality_name);

            if (locality == null) return null; 

            // B. OBTENER/CREAR ADDRESS
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

            // C. DETERMINAR TRAVEL
            int travel_id = await _travel_repository.AssignToExistingOrCreateNewTravelAsync(
                distributionCenterId: 1, 
                transportMethodId: 1 
            );

            // 3. MAPEO A ENTIDAD DE DOMINIO (ShippingDetail)
            var newShipping = new ShippingDetail
            {
                // Corrección: Asignación segura de nullable int a int
                order_id = req.order_id, 
                user_id = req.user_id,  
                
                // Claves Foráneas
                travel_id = travel_id,
                delivery_address_id = delivery_address_id, 
                
                // Mapeo de Value Objects (CORRECCIÓN: Llamada al constructor posicional)
                products = req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList(),

                // Datos de Control (CORRECCIÓN: Conversión de double a float)
                status = ShippingStatus.created,
                total_cost = (float)cost.total_cost, // Conversión explícita de double a float
                currency = cost.currency,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                
                // CORRECCIÓN FINAL: Usa el valor por defecto de días
                estimated_delivery_at = DateTime.UtcNow.AddDays(default_estimated_days), 
                
                tracking_number = Guid.NewGuid().ToString(),
                carrier_name = "PENDIENTE",
                logs = new List<ShippingLog>(new[] { new ShippingLog(DateTime.UtcNow, ShippingStatus.created, "Shipping created in DB.") })
            };

            // 4. PERSISTENCIA EN BD
            await _shipping_repository.AddAsync(newShipping);

            // 5. MAPEO A DTO DE RESPUESTA (CORRECCIÓN: Uso de constructor posicional)
            return new CreateShippingResponse(
                shipping_id: newShipping.shipping_id, 
                status: newShipping.status,
                transport_type: req.transport_type, 
                estimated_delivery_at: newShipping.estimated_delivery_at
            );
        }
    }
}