using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping/cost")]
    public class ShippingCostController : ControllerBase
    {
        private readonly ICalculateCost _calculateCost;
        public ShippingCostController(ICalculateCost calculateCost) => _calculateCost = calculateCost;

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ShippingCostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public ActionResult<ShippingCostResponse> Post([FromBody] ShippingCostRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error { code = "bad_request", message = "Malformed request body." });

            var costReq = new ShippingCostRequest
            (
                request.delivery_address,
                request.products
            );
            
            var cost = _calculateCost.CalculateShippingCost(costReq);

            return Ok(cost);
        }
    }
}
