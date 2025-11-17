using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs.External;

namespace ApiDePapas.Controllers;

/// <summary>
/// Controlador de ejemplo que integra con las APIs de Compras y Stock
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IntegracionController : ControllerBase
{
    private readonly IComprasApiClient _comprasClient;
    private readonly IStockApiClient _stockClient;
    private readonly ILogger<IntegracionController> _logger;

    public IntegracionController(
        IComprasApiClient comprasClient,
        IStockApiClient stockClient,
        ILogger<IntegracionController> logger)
    {
        _comprasClient = comprasClient;
        _stockClient = stockClient;
        _logger = logger;
    }

    /// <summary>
    /// Ejemplo: Obtiene información de una orden de compra
    /// </summary>
    [HttpGet("orden/{ordenId}")]
    [ProducesResponseType(typeof(OrdenCompraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenCompraResponse>> GetOrdenCompra(int ordenId)
    {
        try
        {
            var orden = await _comprasClient.GetOrdenCompraAsync(ordenId);
            
            if (orden == null)
            {
                return NotFound(new { message = $"Orden {ordenId} no encontrada" });
            }

            return Ok(orden);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo orden {OrdenId}", ordenId);
            return StatusCode(500, new { message = "Error al comunicarse con la API de Compras" });
        }
    }

    /// <summary>
    /// Ejemplo: Obtiene el stock de un producto
    /// </summary>
    [HttpGet("stock/{productoId}")]
    [ProducesResponseType(typeof(StockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockResponse>> GetStock(int productoId)
    {
        try
        {
            var stock = await _stockClient.GetStockAsync(productoId);
            
            if (stock == null)
            {
                return NotFound(new { message = $"Stock del producto {productoId} no encontrado" });
            }

            return Ok(stock);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo stock del producto {ProductoId}", productoId);
            return StatusCode(500, new { message = "Error al comunicarse con la API de Stock" });
        }
    }

    /// <summary>
    /// Ejemplo: Verifica disponibilidad y crea reserva de stock
    /// </summary>
    [HttpPost("reservar-stock")]
    [ProducesResponseType(typeof(ReservaStockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReservaStockResponse>> ReservarStock(
        [FromBody] ReservaStockRequest request)
    {
        try
        {
            // Primero verificar disponibilidad
            var disponibilidad = await _stockClient.VerificarDisponibilidadAsync(
                request.producto_id, 
                request.cantidad);

            if (!disponibilidad.disponible)
            {
                return BadRequest(new 
                { 
                    message = "Stock insuficiente",
                    disponible = disponibilidad.cantidad_disponible,
                    solicitado = request.cantidad
                });
            }

            // Si hay stock, crear la reserva
            var reserva = await _stockClient.ReservarStockAsync(request);
            
            return Ok(reserva);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reservando stock del producto {ProductoId}", request.producto_id);
            return StatusCode(500, new { message = "Error al reservar stock" });
        }
    }

    /// <summary>
    /// Ejemplo: Obtiene productos y su stock
    /// </summary>
    [HttpGet("productos-con-stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductosConStock()
    {
        try
        {
            // Obtener productos de la API de Compras
            var productos = await _comprasClient.GetProductosAsync();

            // Para cada producto, obtener su stock
            var productosConStock = new List<object>();

            foreach (var producto in productos.Take(10)) // Limitar a 10 para el ejemplo
            {
                try
                {
                    var stock = await _stockClient.GetStockAsync(producto.id);
                    
                    productosConStock.Add(new
                    {
                        producto.id,
                        producto.nombre,
                        producto.precio,
                        stock = stock != null ? new
                        {
                            stock.cantidad_disponible,
                            stock.cantidad_reservada
                        } : null
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo obtener stock del producto {ProductoId}", producto.id);
                    
                    productosConStock.Add(new
                    {
                        producto.id,
                        producto.nombre,
                        producto.precio,
                        stock = (object?)null
                    });
                }
            }

            return Ok(new
            {
                total = productosConStock.Count,
                productos = productosConStock
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo productos con stock");
            return StatusCode(500, new { message = "Error al obtener información de productos" });
        }
    }

    /// <summary>
    /// Ejemplo: Workflow completo - Crear orden y reservar stock
    /// </summary>
    [HttpPost("procesar-orden")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ProcesarOrden([FromBody] CrearOrdenCompraRequest request)
    {
        try
        {
            _logger.LogInformation("Procesando orden para usuario {UsuarioId}", request.usuario_id);

            // 1. Verificar disponibilidad de todos los productos
            foreach (var item in request.items)
            {
                var disponibilidad = await _stockClient.VerificarDisponibilidadAsync(
                    item.producto_id, 
                    item.cantidad);

                if (!disponibilidad.disponible)
                {
                    return BadRequest(new
                    {
                        message = $"Stock insuficiente para producto {item.producto_id}",
                        producto_id = item.producto_id,
                        disponible = disponibilidad.cantidad_disponible,
                        solicitado = item.cantidad
                    });
                }
            }

            // 2. Crear la orden de compra
            var orden = await _comprasClient.CrearOrdenCompraAsync(request);
            _logger.LogInformation("Orden {OrdenId} creada exitosamente", orden.id);

            // 3. Reservar stock de cada producto
            var reservas = new List<ReservaStockResponse>();
            
            foreach (var item in request.items)
            {
                var reserva = await _stockClient.ReservarStockAsync(new ReservaStockRequest
                {
                    producto_id = item.producto_id,
                    cantidad = item.cantidad,
                    motivo = $"Orden de compra {orden.id}"
                });

                reservas.Add(reserva);
                _logger.LogInformation("Stock reservado: Reserva {ReservaId} para producto {ProductoId}", 
                    reserva.reserva_id, item.producto_id);
            }

            return Ok(new
            {
                message = "Orden procesada exitosamente",
                orden = new
                {
                    orden.id,
                    orden.estado,
                    orden.total,
                    orden.fecha_creacion
                },
                reservas = reservas.Select(r => new
                {
                    r.reserva_id,
                    r.producto_id,
                    r.cantidad
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando orden");
            return StatusCode(500, new { message = "Error al procesar la orden" });
        }
    }
}
