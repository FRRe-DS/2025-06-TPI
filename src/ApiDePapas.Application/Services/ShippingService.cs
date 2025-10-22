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
        
        public async Task<ShippingDetail?> GetAsync(int id)
        => await _shipping_repository.GetByIdAsync(id); // con Includes correctos en el repo

        public async Task<CancelShippingResponse> CancelAsync(int id, DateTime whenUtc)
        {
            // 1) leer de DB (heredado de IGenericRepository)
            var s = await _shipping_repository.GetByIdAsync(id);
            if (s is null)
                throw new KeyNotFoundException($"Shipping {id} not found");

            // 2) validar estado en el servicio (opcional si lo validarás en el repo)
            if (s.status is ShippingStatus.delivered or ShippingStatus.cancelled)
                throw new InvalidOperationException(
                    $"Shipping {id} cannot be cancelled in state '{s.status}'.");

            // 3) persistir el cambio de estado (el repo también agregará el log)
            await _shipping_repository.UpdateStatusAsync(id, ShippingStatus.cancelled);

            // 4) respuesta
            return new CancelShippingResponse(
                shipping_id: id,
                status: ShippingStatus.cancelled,
                cancelled_at: whenUtc
            );
        }


        public async Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest req)
        {
            // 1. Validación Mínima y Cálculo de Costo
            // (A) VALIDACIÓN BÁSICA → 400 con código específico
            if (req == null)
                throw new ArgumentException("bad_request: Body requerido.");

            if (req.products == null || req.products.Count == 0)
                throw new ArgumentException("products_empty: La lista de productos no puede estar vacía.");

            // 1. Cotización (igual que antes)
            var costReq = new ShippingCostRequest(
                req.delivery_address,
                req.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
            );
            var cost = _calculate_cost.CalculateShippingCost(costReq);
            int default_estimated_days = 3;
            // **IMPORTANTE: Si 'cost' NO contiene los días, usaremos un valor por defecto.**
            // Asumo que quieres usar 3 días, como en tu código original.

            // 2. LÓGICA DE NORMALIZACIÓN Y FKs
            
            // A. VALIDAR LOCALITY
            var locality = await _locality_repository.GetByCompositeKeyAsync(
                req.delivery_address.postal_code, 
                req.delivery_address.locality_name);

            if (locality == null)
            throw new ArgumentException(
            $"invalid_locality: La localidad '{req.delivery_address.locality_name}' no pertenece al CP '{req.delivery_address.postal_code}'.");
 

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
        public async Task<ShippingListResponse> List(
            int? userId, ShippingStatus? status, DateOnly? fromDate, DateOnly? toDate, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;

            var all = await _shipping_repository.GetAllAsync();

            // filtros
            if (userId.HasValue) all = all.Where(s => s.user_id == userId.Value);
            if (status.HasValue) all = all.Where(s => s.status == status.Value);
            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                all = all.Where(s => s.created_at >= from);
            }
            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
                all = all.Where(s => s.created_at <= to);
            }

            // orden + paginado
            var ordered = all.OrderByDescending(s => s.created_at);
            var total = ordered.Count();
            var totalPages = (int)Math.Ceiling(total / (double)limit);
            var slice = ordered.Skip((page - 1) * limit).Take(limit).ToList();

            // ⬅️ OJO: tu ShipmentSummary actual espera ENUMS (ShippingStatus y TransportType), no strings
            var summaries = slice.Select(s => new ShipmentSummary(
                ShippingId: s.shipping_id,
                OrderId:    s.order_id,
                UserId:     s.user_id,
                Products:   s.products?.ToList() ?? new List<ProductQty>(),
                Status:     s.status, // enum ShippingStatus (evita el error “cannot convert string → ShippingStatus”)
                TransportType: (s.Travel != null && s.Travel.TransportMethod != null)
                                ? s.Travel.TransportMethod.transport_type
                                : TransportType.truck,  // valor por defecto si faltan las navs
                EstimatedDeliveryAt: s.estimated_delivery_at,
                CreatedAt:           s.created_at
            )).ToList();

            // ⬅️ PaginationData es posicional: (current_page, total_pages, total_items, items_per_page)
            var pagination = new PaginationData(
                current_page:  page,
                total_pages:   totalPages,
                total_items:   total,
                items_per_page:limit
            );

            return new ShippingListResponse(summaries, pagination);
        }
    }
}