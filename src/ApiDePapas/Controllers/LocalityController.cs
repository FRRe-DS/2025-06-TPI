using Microsoft.AspNetCore.Mvc;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Domain.Entities;
using System.Threading.Tasks;

namespace ApiDePapas.Controllers
{
    // Nombre del Controller y Ruta Base
    [ApiController]
    [Route("locality")]
    public class LocalityController : ControllerBase
    {
        // Inyectamos la interfaz del Repositorio
        private readonly ILocalityRepository _locality_repository;

        // Inyección de dependencias (constructor)
        public LocalityController(ILocalityRepository localityRepository)
        {
            _locality_repository = localityRepository;
        }

        /// <summary>
        /// Obtiene los detalles geográficos de una localidad usando su clave compuesta.
        /// </summary>
        /// <param name="postal_code">Código postal de la localidad.</param>
        /// <param name="locality_name">Nombre de la localidad.</param>
        /// <returns>La entidad Locality completa.</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Locality), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLocality(
            // Las variables de entrada usan snake_case, como pediste
            [FromQuery] string postal_code, 
            [FromQuery] string locality_name)
        {
            // 1. Validación de Entrada
            if (string.IsNullOrEmpty(postal_code) || string.IsNullOrEmpty(locality_name))
            {
                // Devolvemos un 400 Bad Request si faltan parámetros
                return BadRequest(new { 
                    code = "missing_parameters", 
                    message = "Se requieren 'postal_code' y 'locality_name' en la consulta." 
                });
            }

            // 2. Consulta al Repositorio (Acceso a la DB)
            var locality = await _locality_repository.GetByCompositeKeyAsync(postal_code, locality_name);

            // 3. Manejo de Respuesta
            if (locality == null)
            {
                // Devolvemos un 404 Not Found
                return NotFound(new {
                    code = "not_found",
                    message = $"Localidad con código postal '{postal_code}' y nombre '{locality_name}' no encontrada."
                });
            }

            // Devolvemos la instancia de Locality completa
            return Ok(locality);
        }
    }
}