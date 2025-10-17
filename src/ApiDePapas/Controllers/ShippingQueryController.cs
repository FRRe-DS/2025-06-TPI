// ApiDePapas/Controllers/ShippingQueryController.cs
using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingQueryController : ControllerBase
    {
        private readonly IShippingStore _store;

        public ShippingQueryController(IShippingStore store)
        {
            _store = store;
        }

        [HttpGet("{id:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateShippingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public ActionResult<CreateShippingResponse> GetById([FromRoute] int id)
        {
            var data = _store.GetById(id);
            if (data is null)
            {
                return NotFound(new Error
                {
                    code = "not_found",
                    message = $"Shipping {id} not found."
                });
            }
            return Ok(data);
        }
    }
}
