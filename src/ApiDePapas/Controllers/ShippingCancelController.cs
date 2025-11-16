// ApiDePapas/Controllers/ShippingCancelController.cs
using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Application.Interfaces; // Para IShippingService
using ApiDePapas.Application.DTOs;      // Para los DTOs
using System.Threading.Tasks;           // Para async Task
using System;                          // Para Exception
using Microsoft.AspNetCore.Http;        // Para StatusCodes

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    [Produces("application/json")]
    public class ShippingCancelController : ControllerBase
    {
        // Inyectamos el SERVICIO
        private readonly IShippingService _shippingService;

        public ShippingCancelController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // ---
        // MÉTODO DE CANCELACIÓN (PATCH)
        // ---
        [HttpPatch("{id:int}/cancel")]
        [ProducesResponseType(typeof(CancelShippingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CancelShippingResponse>> Cancel([FromRoute] int id)
        {
            // El controller es "tonto": solo llama al servicio y traduce errores.
            try
            {
                // 1. Llama a la lógica de negocio en el servicio
                var resp = await _shippingService.CancelShippingAsync(id);
                return Ok(resp);
            }
            // 2. Traduce la excepción "Not Found"
            catch (Exception ex) when (ex.Message.Contains("not found")) 
            {
                return NotFound(new Error 
                { 
                    code = "not_found", 
                    message = ex.Message 
                });
            }
            // 3. Traduce la excepción "Conflict"
            catch (Exception ex) when (ex.Message.Contains("cannot be cancelled"))
            {
                return Conflict(new Error 
                { 
                    code = "conflict", 
                    message = ex.Message 
                });
            }
            // 4. Captura cualquier otro error inesperado
            catch (Exception ex)
            {
                return StatusCode(500, new Error { code = "server_error", message = ex.Message });
            }
        }
    }
}