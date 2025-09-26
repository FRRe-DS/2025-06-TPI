using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Services;
using ApiDePapas.Models;

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
            var response = _calculateCost.CalculateShippingCost(new ShippingCostRequest
            {
                delivery_address = request.delivery_address,
                departure_postal_code = request.departure_postal_code,
                products = request.products
            });

            return Ok(response);
        }
    }
}


