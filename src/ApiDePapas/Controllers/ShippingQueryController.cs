using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Utils;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingQueryController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingQueryController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // ---
        // MÉTODO 1: Implementa 'GET /shipping' (con filtros)
        // ---
        [HttpGet]
        [ProducesResponseType(typeof(ShippingListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShippingListResponse>> Get(
            [FromQuery(Name = "user_id")] int? userId,
            [FromQuery(Name = "status")] string? statusStr,
            [FromQuery(Name = "from_date")] DateOnly? fromDate,
            [FromQuery(Name = "to_date")] DateOnly? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)

            
        {
            // Parsing tolerante del status
            if (!ShippingStatusParser.TryParse(statusStr, out var status))
            {
                return BadRequest(new Error { code = "invalid_status", message = "status inválido." });
            }

            // Validación de fechas
            if (fromDate.HasValue && toDate.HasValue && toDate < fromDate)
                return BadRequest(new Error { code = "invalid_range", message = "to_date debe ser >= from_date." });

            // Delegación total al servicio
            var result = await _shippingService.List(userId, status, fromDate, toDate, page, limit);
            return Ok(result);
        }

        // ---
        // MÉTODO 2: Implementa 'GET /shipping/{shipping_id}'
        // ---
        [HttpGet("{shipping_id:int}")] 
        [Produces("application/json")]
        [ProducesResponseType(typeof(ShippingDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShippingDetailResponse>> GetById([FromRoute] int shipping_id)
        {
            // Delegación total al servicio
            var responseDto = await _shippingService.GetByIdAsync(shipping_id); 
            
            // Chequeo de null
            if (responseDto is null)
            {
                return NotFound(new Error
                {
                    code = "not_found",
                    message = $"Shipping {shipping_id} not found."
                });
            }

            // 3. Retorno del DTO
            return Ok(responseDto);
        }
    }
}