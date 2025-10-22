using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Services;

namespace ApiDePapas.Controllers
{
    // En ShippingCreateController.cs (versión refactorizada)   
    [ApiController]
    [Route("shipping")]
    public class ShippingCreateController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingCreateController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateShippingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateShippingResponse>> Post([FromBody] CreateShippingRequest req)
        {
            if (!ModelState.IsValid)
        return BadRequest(new Error { code = "bad_request", message = "Malformed request body." });

            try
            {
                var created = await _shippingService.CreateNewShipping(req);
                // Si el servicio llegó aquí, no devolvió null ni lanzó excepción → OK
                return Created($"/shipping/{created!.shipping_id}", created);
            }
            catch (ArgumentException ex)
            {
                // Esperamos mensajes con el patrón "code: mensaje"
                var msg = ex.Message;
                var sep = msg.IndexOf(':');
                var code = sep > 0 ? msg[..sep].Trim() : "bad_request";
                var text = sep > 0 ? msg[(sep + 1)..].Trim() : msg;

                return BadRequest(new Error { code = code, message = text });
            }
        }
    }
}