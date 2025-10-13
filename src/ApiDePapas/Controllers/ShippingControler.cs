using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;

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

        [HttpPost("cost")]
        public ActionResult<ShippingCostResponse> PostCost([FromBody] ShippingCostRequest request)
        {
            if (request == null || request.products == null || request.products.Count == 0)
                return BadRequest("Request inválido");

            // Llamamos al servicio y devolvemos el resultado completo
            var response = _calculateCost.CalculateShippingCost(request);

            return Ok(response);
        }
    }
}


