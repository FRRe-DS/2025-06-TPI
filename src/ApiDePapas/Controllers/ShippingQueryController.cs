using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Domain.Repositories; 
using ApiDePapas.Application.DTOs; 
using ApiDePapas.Domain.Entities; 
using System.Threading.Tasks;
using System.Linq;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingQueryController : ControllerBase
    {
        private readonly IShippingRepository _shipping_repository;

        public ShippingQueryController(IShippingRepository shippingRepository)
        {
            _shipping_repository = shippingRepository;
        }

        [HttpGet("{id:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ShippingDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShippingDetailResponse>> GetById([FromRoute] int id)
        {
            // 1. Obtener la entidad COMPLETA de la BD 
            var data = await _shipping_repository.GetByIdAsync(id);
            
            if (data is null)
            {
                return NotFound(new Error
                {
                    code = "not_found",
                    message = $"Shipping {id} not found."
                });
            }

            // Mapeo del DistributionCenter Address para llenar el campo departure_address
            // CORRECCIÓN: Usamos data.travel.DistributionCenter.Address
            var departureAddressEntity = data.Travel.DistributionCenter.Address;

            // 2. Mapeo a DTO con nombres EXACTOS del YAML
            var responseDto = new ShippingDetailResponse
            {
                // Propiedades Simples
                shipping_id = data.shipping_id,
                order_id = data.order_id,
                user_id = data.user_id,
                status = data.status,
                tracking_number = data.tracking_number,
                carrier_name = data.carrier_name,
                total_cost = data.total_cost,
                currency = data.currency,
                estimated_delivery_at = data.estimated_delivery_at,
                created_at = data.created_at,
                updated_at = data.updated_at,
                
                // Mapeo de ENUM a STRING
                transport_type = data.Travel.TransportMethod.transport_type.ToString(), 

                // Domicilios - Usamos los nombres EXACTOS del YAML
                delivery_address = new AddressReadDto // Coincide con delivery_address del YAML
                {
                    address_id = data.DeliveryAddress.address_id,
                    street = data.DeliveryAddress.street,
                    number = data.DeliveryAddress.number,
                    postal_code = data.DeliveryAddress.postal_code,
                    locality_name = data.DeliveryAddress.locality_name,
                },
                // Mapeo del DEPARTURE ADDRESS (Origen del viaje)
                departure_address = new AddressReadDto 
                {
                    // CORRECCIÓN: Usamos address_id de la entidad DistributionCenter
                    address_id = data.Travel.DistributionCenter.address_id, 
                    // Mapeamos los detalles de la dirección cargada
                    street = departureAddressEntity.street,
                    number = departureAddressEntity.number,
                    postal_code = departureAddressEntity.postal_code,
                    locality_name = departureAddressEntity.locality_name,
                },
                
                // Colecciones (Logs y Products)
                products = data.products.Select(p => new ProductQtyReadDto(p.id, p.quantity)).ToList(),
                logs = data.logs.Select(l => new ShippingLogReadDto(l.Timestamp ?? DateTime.MinValue, l.Status ?? ShippingStatus.created, l.Message)).ToList()
            };
            
            return Ok(responseDto);
        }
    }
}