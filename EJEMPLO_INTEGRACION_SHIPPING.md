# 💡 Ejemplo de Integración en ShippingService

Este documento muestra cómo integrar las APIs de Compras y Stock en tu servicio `ShippingService` existente.

## Escenario: Crear un envío verificando stock

Cuando un usuario crea un envío, necesitas:
1. Verificar que hay stock disponible de los productos
2. Reservar el stock
3. Crear el envío
4. Si algo falla, liberar la reserva

---

## Modificación del ShippingService

```csharp
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.DTOs.External;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ApiDePapas.Application.Services;

public class ShippingService : IShippingService
{
    private readonly IShippingRepository _shippingRepository;
    private readonly IStockApiClient _stockClient;  // ← NUEVO
    private readonly IComprasApiClient _comprasClient;  // ← NUEVO
    private readonly ICalculateCost _calculateCost;
    private readonly ILogger<ShippingService> _logger;

    public ShippingService(
        IShippingRepository shippingRepository,
        IStockApiClient stockClient,  // ← INYECTAR
        IComprasApiClient comprasClient,  // ← INYECTAR
        ICalculateCost calculateCost,
        ILogger<ShippingService> logger)
    {
        _shippingRepository = shippingRepository;
        _stockClient = stockClient;
        _comprasClient = comprasClient;
        _calculateCost = calculateCost;
        _logger = logger;
    }

    public async Task<Shipping> CreateNewShipping(CreateShippingRequest req)
    {
        _logger.LogInformation("Creando envío para usuario {UserId}, orden {OrderId}", 
            req.user_id, req.order_id);

        // ═══════════════════════════════════════════════════════════
        // PASO 1: OBTENER INFORMACIÓN DE LA ORDEN DE COMPRAS
        // ═══════════════════════════════════════════════════════════
        OrdenCompraResponse? orden = null;
        
        try
        {
            orden = await _comprasClient.GetOrdenCompraAsync(req.order_id);
            
            if (orden == null)
            {
                _logger.LogWarning("Orden {OrderId} no encontrada en API de Compras", req.order_id);
                throw new InvalidOperationException($"Orden {req.order_id} no encontrada");
            }

            _logger.LogInformation("Orden {OrderId} encontrada: {ItemCount} items, total ${Total}", 
                orden.id, orden.items.Count, orden.total);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error comunicándose con API de Compras");
            throw new InvalidOperationException("No se pudo verificar la orden de compra. Intente nuevamente.", ex);
        }

        // ═══════════════════════════════════════════════════════════
        // PASO 2: VERIFICAR STOCK DE TODOS LOS PRODUCTOS
        // ═══════════════════════════════════════════════════════════
        var productosConStock = new Dictionary<int, StockResponse>();
        
        foreach (var item in orden.items)
        {
            try
            {
                var stock = await _stockClient.GetStockAsync(item.producto_id);
                
                if (stock == null)
                {
                    _logger.LogWarning("Producto {ProductoId} no tiene registro de stock", item.producto_id);
                    throw new InvalidOperationException($"Producto {item.producto_id} sin stock disponible");
                }

                if (stock.cantidad_disponible < item.cantidad)
                {
                    _logger.LogWarning("Stock insuficiente para producto {ProductoId}: disponible {Disponible}, requerido {Requerido}",
                        item.producto_id, stock.cantidad_disponible, item.cantidad);
                    
                    throw new InvalidOperationException(
                        $"Stock insuficiente para producto {item.producto_id}. " +
                        $"Disponible: {stock.cantidad_disponible}, Requerido: {item.cantidad}");
                }

                productosConStock[item.producto_id] = stock;
                _logger.LogInformation("Stock OK para producto {ProductoId}: {Disponible} unidades disponibles", 
                    item.producto_id, stock.cantidad_disponible);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error comunicándose con API de Stock");
                throw new InvalidOperationException("No se pudo verificar el stock. Intente nuevamente.", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        // PASO 3: RESERVAR STOCK DE TODOS LOS PRODUCTOS
        // ═══════════════════════════════════════════════════════════
        var reservasCreadas = new List<ReservaStockResponse>();
        
        try
        {
            foreach (var item in orden.items)
            {
                var reserva = await _stockClient.ReservarStockAsync(new ReservaStockRequest
                {
                    producto_id = item.producto_id,
                    cantidad = item.cantidad,
                    motivo = $"Envío para orden {req.order_id} - Usuario {req.user_id}"
                });

                reservasCreadas.Add(reserva);
                
                _logger.LogInformation("Reserva {ReservaId} creada para producto {ProductoId}: {Cantidad} unidades",
                    reserva.reserva_id, reserva.producto_id, reserva.cantidad);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reservar stock. Liberando {Count} reservas creadas", reservasCreadas.Count);
            
            // ROLLBACK: Liberar todas las reservas creadas hasta ahora
            foreach (var reserva in reservasCreadas)
            {
                try
                {
                    await _stockClient.LiberarReservaAsync(reserva.reserva_id);
                    _logger.LogInformation("Reserva {ReservaId} liberada exitosamente", reserva.reserva_id);
                }
                catch (Exception liberarEx)
                {
                    _logger.LogError(liberarEx, "Error al liberar reserva {ReservaId}", reserva.reserva_id);
                }
            }

            throw new InvalidOperationException("Error al reservar stock para el envío", ex);
        }

        // ═══════════════════════════════════════════════════════════
        // PASO 4: CREAR EL ENVÍO EN TU BASE DE DATOS
        // ═══════════════════════════════════════════════════════════
        try
        {
            // Convertir items de la orden a ProductQty de tu dominio
            var products = req.products ?? orden.items.Select(i => new ProductQty
            {
                id = i.producto_id,
                quantity = i.cantidad
            }).ToList();

            // Calcular costo de envío
            var costRequest = new ShippingCostRequest
            {
                address = req.address,
                products = products
            };
            
            var costResponse = _calculateCost.Calculate(costRequest);

            // Crear el envío
            var shipping = new Shipping
            {
                order_id = req.order_id,
                user_id = req.user_id,
                products = products,
                address = req.address,
                status = "pending",
                estimated_delivery_at = DateTime.UtcNow.AddDays(costResponse.estimated_days),
                created_at = DateTime.UtcNow
                // ... otros campos
            };

            await _shippingRepository.AddAsync(shipping);
            await _shippingRepository.SaveChangesAsync();

            _logger.LogInformation("Envío {ShippingId} creado exitosamente para orden {OrderId}. Reservas: [{Reservas}]",
                shipping.shipping_id, 
                req.order_id,
                string.Join(", ", reservasCreadas.Select(r => r.reserva_id)));

            // TODO: Guardar IDs de reservas en una tabla relacionada para poder liberarlas después
            // si se cancela el envío

            return shipping;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear envío. Liberando {Count} reservas", reservasCreadas.Count);
            
            // ROLLBACK: Liberar todas las reservas
            foreach (var reserva in reservasCreadas)
            {
                try
                {
                    await _stockClient.LiberarReservaAsync(reserva.reserva_id);
                    _logger.LogInformation("Reserva {ReservaId} liberada después de error", reserva.reserva_id);
                }
                catch (Exception liberarEx)
                {
                    _logger.LogError(liberarEx, "Error al liberar reserva {ReservaId}", reserva.reserva_id);
                }
            }

            throw;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // MÉTODO PARA CANCELAR ENVÍO Y LIBERAR STOCK
    // ═══════════════════════════════════════════════════════════
    public async Task<bool> CancelShippingAsync(int shippingId)
    {
        _logger.LogInformation("Cancelando envío {ShippingId}", shippingId);

        var shipping = await _shippingRepository.GetByIdAsync(shippingId);
        
        if (shipping == null)
        {
            _logger.LogWarning("Envío {ShippingId} no encontrado", shippingId);
            return false;
        }

        if (shipping.status == "cancelled")
        {
            _logger.LogWarning("Envío {ShippingId} ya está cancelado", shippingId);
            return false;
        }

        // TODO: Obtener los IDs de reservas asociados al envío desde tu DB
        // Por ahora, asumimos que no tenemos esa información
        // En una implementación real, deberías guardar los reservation_ids cuando creas el envío

        // Actualizar estado del envío
        shipping.status = "cancelled";
        shipping.updated_at = DateTime.UtcNow;
        
        await _shippingRepository.UpdateAsync(shipping);
        await _shippingRepository.SaveChangesAsync();

        _logger.LogInformation("Envío {ShippingId} cancelado exitosamente", shippingId);

        // Nota: En producción, deberías liberar las reservas de stock aquí
        // await _stockClient.LiberarReservaAsync(reservationId);

        return true;
    }
}
```

---

## Tabla para Guardar Reservas de Stock

Para poder liberar las reservas cuando se cancela un envío, necesitas guardar los IDs:

```sql
CREATE TABLE shipping_stock_reservations (
    id INT PRIMARY KEY AUTO_INCREMENT,
    shipping_id INT NOT NULL,
    reservation_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    created_at DATETIME NOT NULL,
    released_at DATETIME NULL,
    FOREIGN KEY (shipping_id) REFERENCES shippings(shipping_id),
    INDEX idx_shipping_id (shipping_id)
);
```

Y la entidad en C#:

```csharp
namespace ApiDePapas.Domain.Entities;

public class ShippingStockReservation
{
    public int id { get; set; }
    public int shipping_id { get; set; }
    public int reservation_id { get; set; }  // ID de la reserva en la API de Stock
    public int producto_id { get; set; }
    public int cantidad { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? released_at { get; set; }
}
```

---

## Flujo Completo con Manejo de Errores

```
┌─────────────────────────────────────────────┐
│ Usuario: POST /api/shipping                 │
└────────────────┬────────────────────────────┘
                 │
                 ▼
    ┌────────────────────────────┐
    │ 1. Obtener orden de Compras│
    └────────┬───────────────────┘
             │ ✅ Success
             ▼
    ┌────────────────────────────┐
    │ 2. Verificar stock (loop)  │
    └────────┬───────────────────┘
             │ ✅ Todos tienen stock
             ▼
    ┌────────────────────────────┐
    │ 3. Reservar stock (loop)   │◄──┐
    └────────┬───────────────────┘   │
             │ ✅ Todas reservadas   │ ❌ Error
             ▼                        │
    ┌────────────────────────────┐   │
    │ 4. Crear envío en DB       │   │
    └────────┬───────────────────┘   │
             │ ✅ Creado             │
             ▼                        │
    ┌────────────────────────────┐   │
    │ 5. Guardar reservation_ids │   │
    └────────┬───────────────────┘   │
             │                        │
             ▼                        │
    ┌────────────────────────────┐   │
    │ 6. Retornar respuesta      │   │
    └────────────────────────────┘   │
                                      │
                                      │
    ┌────────────────────────────┐   │
    │ ROLLBACK:                  │───┘
    │ - Liberar todas las        │
    │   reservas creadas         │
    │ - Retornar error al usuario│
    └────────────────────────────┘
```

---

## Ejemplo de Uso en el Controlador

```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<CreateShippingResponse>> CreateShipping(
    [FromBody] CreateShippingRequest request)
{
    try
    {
        var shipping = await _shippingService.CreateNewShipping(request);
        
        return Ok(new CreateShippingResponse
        {
            shipping_id = shipping.shipping_id,
            status = shipping.status,
            estimated_delivery_at = shipping.estimated_delivery_at,
            message = "Envío creado exitosamente. Stock reservado."
        });
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Stock insuficiente"))
    {
        return BadRequest(new { error = "stock_insuficiente", message = ex.Message });
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Orden") && ex.Message.Contains("no encontrada"))
    {
        return NotFound(new { error = "orden_no_encontrada", message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return StatusCode(503, new { error = "servicio_no_disponible", message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error inesperado al crear envío");
        return StatusCode(500, new { error = "error_interno", message = "Error al crear el envío" });
    }
}
```

---

## Testing

Para probar la integración, puedes crear un test de integración:

```csharp
[Fact]
public async Task CreateShipping_ConStockDisponible_CreaEnvioYReservaStock()
{
    // Arrange
    var request = new CreateShippingRequest
    {
        order_id = 123,
        user_id = 1,
        products = new List<ProductQty>
        {
            new() { id = 101, quantity = 2 }
        },
        address = new AddressRequest { /* ... */ }
    };

    // Mock de ComprasApiClient: orden existe
    _mockComprasClient
        .Setup(c => c.GetOrdenCompraAsync(123))
        .ReturnsAsync(new OrdenCompraResponse
        {
            id = 123,
            items = new List<ItemCompra>
            {
                new() { producto_id = 101, cantidad = 2 }
            }
        });

    // Mock de StockApiClient: hay stock
    _mockStockClient
        .Setup(s => s.GetStockAsync(101))
        .ReturnsAsync(new StockResponse
        {
            producto_id = 101,
            cantidad_disponible = 50,
            cantidad_reservada = 10
        });

    // Mock de StockApiClient: reserva exitosa
    _mockStockClient
        .Setup(s => s.ReservarStockAsync(It.IsAny<ReservaStockRequest>()))
        .ReturnsAsync(new ReservaStockResponse
        {
            reserva_id = 999,
            producto_id = 101,
            cantidad = 2,
            estado = "reservado"
        });

    // Act
    var result = await _shippingService.CreateNewShipping(request);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(123, result.order_id);
    Assert.Equal("pending", result.status);
    
    // Verificar que se llamó a reservar stock
    _mockStockClient.Verify(
        s => s.ReservarStockAsync(It.Is<ReservaStockRequest>(r => 
            r.producto_id == 101 && r.cantidad == 2)), 
        Times.Once);
}
```

---

## Checklist de Implementación

- [ ] Inyectar `IComprasApiClient` y `IStockApiClient` en `ShippingService`
- [ ] Modificar `CreateNewShipping` para verificar y reservar stock
- [ ] Implementar lógica de rollback si falla algún paso
- [ ] Crear tabla `shipping_stock_reservations` para guardar IDs de reservas
- [ ] Modificar `CancelShipping` para liberar reservas de stock
- [ ] Agregar logs detallados en cada paso
- [ ] Manejar errores específicos (stock insuficiente, orden no encontrada, API no disponible)
- [ ] Crear tests unitarios con mocks
- [ ] Crear tests de integración con APIs reales
- [ ] Documentar el flujo para otros desarrolladores

---

¡Con esta implementación, tu API de Logística estará completamente integrada con las APIs de Compras y Stock! 🚀
