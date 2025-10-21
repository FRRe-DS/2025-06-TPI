using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingController : ControllerBase
    {
        private readonly ICalculateCost _calculateCost;
        private readonly IShippingService _shippingService;

        public ShippingController(ICalculateCost calculateCost, IShippingService shippingService)
        {
            _calculateCost = calculateCost;
            _shippingService = shippingService;
        }

        // -----------------------------
        // POST /shipping/cost
        // -----------------------------
        [HttpPost("cost")]
        public ActionResult<ShippingCostResponse> PostCost([FromBody] ShippingCostRequest request)
        {
            if (request == null || request.products == null || request.products.Count == 0)
                return BadRequest("Request inválido");

            var response = _calculateCost.CalculateShippingCost(request);
            return Ok(response);
        }

        // -----------------------------
        // GET /shipping (listar con filtros)
        // -----------------------------
        [HttpGet]
        [ProducesResponseType(typeof(ShippingListResponse), StatusCodes.Status200OK)]
        public ActionResult<ShippingListResponse> Get(
            [FromQuery(Name = "user_id")] int? userId,
            [FromQuery(Name = "status")] string? statusStr,
            [FromQuery(Name = "from_date")] DateOnly? fromDate,
            [FromQuery(Name = "to_date")] DateOnly? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            // -- Parse "status" tolerante: acepta in_transit, in-transit, InTransit, etc.
            ShippingStatus? status = null;
            if (!string.IsNullOrWhiteSpace(statusStr))
            {
                // 1) normalizo a snake_case: guiones y espacios -> _
                var snake = statusStr.Trim()
                                    .Replace("-", "_")
                                    .Replace(" ", "_")
                                    .ToLowerInvariant();

                // 2) intento parsear tal cual (para enums definidos como in_transit)
                if (!Enum.TryParse<ShippingStatus>(snake, ignoreCase: true, out var parsed))
                {
                    // 3) fallback: convierto snake_case -> PascalCase y vuelvo a intentar
                    var parts  = snake.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    var pascal = string.Concat(parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
                    if (!Enum.TryParse<ShippingStatus>(pascal, ignoreCase: true, out parsed))
                    {
                        return UnprocessableEntity(new Error { code = "invalid_status", message = "status inválido" });
                    }
                    status = parsed;
                }
                else
                {
                    status = parsed;
                }
            }


            if (fromDate.HasValue && toDate.HasValue && toDate < fromDate)
            {
                return UnprocessableEntity(new Error
                {
                    code = "invalid_range",
                    message = "to_date debe ser >= from_date"
                });
            }

            var result = _shippingService.List(userId, status, fromDate, toDate, page, limit);
            return Ok(result);
        }
    }
}
