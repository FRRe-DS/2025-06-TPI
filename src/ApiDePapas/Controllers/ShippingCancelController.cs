// ApiDePapas/Controllers/ShippingCancelController.cs
using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingCancelController : ControllerBase
    {
        private readonly IShippingService _service;

        public ShippingCancelController(IShippingService service)
        {
            _service = service;
        }

        // POST /shipping/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CancelShippingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CancelShippingResponse>> Cancel([FromRoute] int id)
        {
            // 1) buscar en DB
            var shipping = await _service.GetAsync(id);  // <-- método del servicio
            if (shipping is null)
            {
                return NotFound(new Error
                {
                    code = "not_found",
                    message = $"Shipping {id} not found."
                });
            }

            // 2) validar estado
            if (shipping.status is ShippingStatus.delivered or ShippingStatus.cancelled)
            {
                return Conflict(new Error
                {
                    code = "conflict",
                    message = $"Shipping {id} cannot be cancelled in state '{shipping.status}'."
                });
            }

            // 3) cancelar (DB)
            var resp = await _service.CancelAsync(id, DateTime.UtcNow);
            return Ok(resp);
        }
    }
}
