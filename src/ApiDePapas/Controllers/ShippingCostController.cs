using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Repositories;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping/cost")]
    public class ShippingCostController : ControllerBase
    {
        private readonly ICalculateCost _calculateCost;
        private readonly ILocalityRepository _localityRepo;
        public ShippingCostController(ICalculateCost calculateCost, ILocalityRepository localityRepo)
        {
            _calculateCost = calculateCost;
            _localityRepo = localityRepo;
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ShippingCostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShippingCostResponse>> Post([FromBody] ShippingCostRequest request)
        {
            // 1) JSON mal formado / anotaciones del modelo
            if (!ModelState.IsValid)
                return BadRequest(new Error { code = "bad_request", message = "Malformed request body." });

            // 2) Validaciones básicas de entrada (igual criterio que en Create)
            if (request.products == null || request.products.Count == 0)
                return BadRequest(new Error { code = "products_empty", message = "La lista de productos no puede estar vacía." });

            if (request.delivery_address is null ||
                string.IsNullOrWhiteSpace(request.delivery_address.postal_code) ||
                string.IsNullOrWhiteSpace(request.delivery_address.locality_name))
                return BadRequest(new Error { code = "address_incomplete", message = "postal_code y locality_name son obligatorios." });

            // 3) Validar catálogo: la combinación CP + localidad debe existir
            var loc = await _localityRepo.GetByCompositeKeyAsync(
                request.delivery_address.postal_code,
                request.delivery_address.locality_name);

            if (loc is null)
                return BadRequest(new Error
                {
                    code = "invalid_locality",
                    message = $"La localidad '{request.delivery_address.locality_name}' no pertenece al CP '{request.delivery_address.postal_code}'."
                });

            // 4) Calcular y capturar errores de negocio lanzados por CalculateCost
            try
            {
                var cost = _calculateCost.CalculateShippingCost(request);
                return Ok(cost);
            }
            catch (ArgumentException ex)
            {
                // Esperamos "code: mensaje"
                var msg = ex.Message;
                var sep = msg.IndexOf(':');
                var code = sep > 0 ? msg[..sep].Trim() : "bad_request";
                var text = sep > 0 ? msg[(sep + 1)..].Trim() : msg;

                return BadRequest(new Error { code = code, message = text });
            }
        }
    }
}