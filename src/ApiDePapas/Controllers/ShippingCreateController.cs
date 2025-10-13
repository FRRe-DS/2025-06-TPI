using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Services;

namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("shipping")]
    public class ShippingCreateController : ControllerBase
    {
        private readonly IShippingStore _store;
        private readonly ICalculateCost _calculateCost;

        public ShippingCreateController(IShippingStore store, ICalculateCost calculateCost)
        (
            _store = store;
            _calculateCost = calculateCost;
        )

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CreateShippingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public ActionResult<CreateShippingResponse> Post([FromBody] CreateShippingRequest req)
        {
            // 400 - request mal formado (data annotations)
            if (!ModelState.IsValid)
                return BadRequest(new Error { code = "bad_request", message = "Malformed request body." });

            // 422 - validación de negocio
            if (req.products == null || req.products.Count == 0)
            {
                return UnprocessableEntity(new Error
                {
                    code = "unprocessable_entity",
                    message = "Validation failed.",
                    details = new
                    {
                        field_errors = new[]
                        {
                            new { field = "products", message = "Must contain at least 1 item" }
                        }
                    }
                });
            }

            // (Opcional) recalcular/validar costo con el transport_type elegido
            var costReq = new ShippingCostRequest
            (
                req.delivery_address,
                req.products
            );
            _ = _calculateCost.CalculateShippingCost(costReq);

            // Crear el envío (mock/persistencia real según tu store)
            var created = new CreateShippingResponse
            (
                ShippingStatus.created,
                req.transport_type,
                DateTimeOffset.UtcNow.AddDays(3).ToString("O") // ISO 8601 UTC
            );

            var id = _store.Save(created);   // tu store debe devolver el nuevo ID
            created.shipping_id = id;

            // 201 + Location: /shipping/{id}
            return Created($"/shipping/{created.shipping_id}", created);
            // Alternativa equivalente:
            // return CreatedAtAction(nameof(GetById), new { shipping_id = created.shipping_id }, created);
        }
    }
}
