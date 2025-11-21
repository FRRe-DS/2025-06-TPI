using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Utils;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping/filter")]
    public class ShippingQueryFilterController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingQueryFilterController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // GET /shipping/filter?user_id=&status=&from_date=&to_date=&page=&limit=
        [HttpGet]
        [ProducesResponseType(typeof(ShippingListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShippingListResponse>> Get(
            [FromQuery(Name = "user_id")] int? userId,
            [FromQuery(Name = "status")] string? statusStr,
            [FromQuery(Name = "from_date(YYYY-MM-DD)")] DateOnly? fromDate,
            [FromQuery(Name = "to_date(YYYY-MM-DD)")] DateOnly? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            // Parsing tolerante del status
            if (!ShippingStatusParser.TryParse(statusStr, out var status))
            {
                return BadRequest(new Error { code = "invalid_status", message = "status inválido." });
            }

            if (fromDate.HasValue && toDate.HasValue && toDate < fromDate)
            {
                return BadRequest(new Error { code = "invalid_range", message = "to_date debe ser >= from_date." });
            }
                
            // Usa el método List reintroducido en IShippingService
            var result = await _shippingService.List(userId, status, fromDate, toDate, page, limit);
            return Ok(result);
        }
    }
}