// ApiDePapas/Controllers/ShippingCancelController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    [Authorize]
    public class ShippingCancelController : ControllerBase
    {
        private readonly IShippingStore _store;

        public ShippingCancelController(IShippingStore store)
        {
            _store = store;
        }
//DE MOMENTO PUSE COMO PATCH PERO PODRÍA SER POST, EN EL YAML DICE POST
        [HttpPatch("{id:int}/cancel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CancelShippingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public ActionResult<CancelShippingResponse> Cancel([FromRoute] int id)
        {
            var current = _store.GetById(id);
            if (current is null)
            {
                return NotFound(new Error
                {
                    code = "not_found",
                    message = $"Shipping {id} not found."
                });
            }

            if (current.status == ShippingStatus.delivered || current.status == ShippingStatus.cancelled)
            {
                return Conflict(new Error
                {
                    code = "conflict",
                    message = $"Shipping {id} cannot be cancelled in state '{current.status}'."
                });
            }

            var resp = _store.Cancel(id, DateTime.UtcNow);
            return Ok(resp);
        }
    }
}
