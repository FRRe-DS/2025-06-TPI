using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http; // Added for StatusCodes
using ApiDePapas.Domain.Entities;


namespace ApiDePapas.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = "logistica-be")] // Solo accesible para backend de logística
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("shipments")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedDashboardShipmentsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedDashboardShipmentsResponse>> GetDashboardShipments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var shipments = await _dashboardService.GetDashboardShipmentsAsync(page, pageSize);
            var totalItems = await _dashboardService.GetTotalDashboardShipmentsCountAsync();

            var response = new PaginatedDashboardShipmentsResponse(
                shipments,
                new PaginationData(
                    total_items: totalItems,
                    total_pages: (int)Math.Ceiling((double)totalItems / pageSize),
                    current_page: page,
                    items_per_page: pageSize
                )
            );

            return Ok(response);
        }
        [HttpPatch("shipments/{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShipmentStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                await _dashboardService.UpdateShipmentStatusAsync(id, request.NewStatus);
                return NoContent(); // 204 No Content es estándar para updates exitosos sin cuerpo de respuesta
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new Error { code = "not_found", message = ex.Message });
            }
        }
    }
}
