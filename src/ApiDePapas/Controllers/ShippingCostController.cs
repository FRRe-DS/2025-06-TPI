using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Controllers;

[ApiController]
[Route("api/shipping/cost")]
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
    public async Task<ActionResult<ShippingCostResponse>> Post([FromBody] ShippingCostRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new Error { code = "bad_request", message = "Malformed request body." });
        
        var cost = await _calculateCost.CalculateShippingCostAsync(request);

        return Ok(cost);
    }
}