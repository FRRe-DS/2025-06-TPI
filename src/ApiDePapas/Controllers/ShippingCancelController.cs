using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Controllers;

[ApiController]
[Route("api/shipping")]
public class ShippingCancelController : ControllerBase
{
    private readonly IShippingService _shippingService; 

    public ShippingCancelController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    [HttpPost("{id:int}/cancel")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CancelShippingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CancelShippingResponse>> Cancel([FromRoute] int id)
    {
        // Buscar en DB
        var shipping = await _shippingService.GetByIdAsync(id); 
        if (shipping is null)
        {
            return NotFound(new Error
            {
                code = "not_found",
                message = $"Shipping {id} not found."
            });
        }

        // Validar estado
        if (!shipping.IsCancellable())
        {
            return Conflict(new Error
            {
                code = "conflict",
                message = $"Shipping {id} cannot be cancelled in state '{shipping.status}'."
            });
        }

        // Cancelar (en DB)
        var resp = await _shippingService.CancelAsync(id, DateTime.UtcNow);
        return Ok(resp);
    }
}