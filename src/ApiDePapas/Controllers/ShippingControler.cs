using ApiDePapas.Models;
using ApiDePapas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingController : ControllerBase
    {
        private readonly ICalculateCost _calculateCost;

        public ShippingController(ICalculateCost calculateCost)
        {
            _calculateCost = calculateCost;
        }

        /// <summary>
        /// Calcula el costo de un envío
        /// </summary>
        [HttpPost("cost")]
        public IActionResult CalculateCost([FromBody] ShippingCostRequest request)
        {
            if (request == null || request.Products == null || !request.Products.Any())
            {
                return BadRequest(new { message = "Request inválido" });
            }

            try
            {
                var result = _calculateCost.CalculateCost(request);
                return Ok(result);
            }
            catch
            {
                // En caso de error interno
                return StatusCode(500, new { message = "Error calculando el costo" });
            }
        }
    }
}
