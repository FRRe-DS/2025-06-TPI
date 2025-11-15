# 🔗 Guía de Integración con APIs Externas (Compras y Stock)

## 📋 Resumen

Tu API de Logística ahora puede comunicarse con las APIs de **Compras** y **Stock** de otros grupos usando autenticación JWT de Keycloak. La integración incluye:

- ✅ Servicio de tokens de Keycloak con caché automático
- ✅ Cliente HTTP tipado para API de Compras
- ✅ Cliente HTTP tipado para API de Stock
- ✅ Renovación automática de tokens
- ✅ Manejo de errores y logging
- ✅ Controlador de ejemplo con workflows completos

---

## 🏗️ Arquitectura

```
┌─────────────────────┐
│   Tu API Logística  │
│     (Grupo 06)      │
└──────────┬──────────┘
           │
           ├──────────────────────┐
           │                      │
           ▼                      ▼
┌──────────────────┐    ┌──────────────────┐
│  API de Compras  │    │  API de Stock    │
│  (Grupo 01/04..) │    │  (Grupo 02/05..) │
└────────┬─────────┘    └────────┬─────────┘
         │                       │
         └───────────┬───────────┘
                     │
                     ▼
            ┌────────────────┐
            │    Keycloak    │
            │  (Token Server)│
            └────────────────┘
```

**Flujo:**
1. Tu API solicita token a Keycloak con `client_id=grupo-06` y `client_secret`
2. Keycloak valida credenciales y devuelve token JWT
3. Tu API usa el token para llamar a APIs de Compras/Stock
4. El token se cachea y renueva automáticamente antes de expirar

---

## 📁 Archivos Creados

### **DTOs (Data Transfer Objects)**

```
src/ApiDePapas.Application/DTOs/External/
├── ComprasApiDTOs.cs      # DTOs para API de Compras
├── StockApiDTOs.cs        # DTOs para API de Stock
└── KeycloakDTOs.cs        # DTOs para respuestas de Keycloak
```

### **Interfaces**

```
src/ApiDePapas.Application/Interfaces/
├── IKeycloakTokenService.cs  # Servicio de tokens
├── IComprasApiClient.cs      # Cliente de Compras
└── IStockApiClient.cs        # Cliente de Stock
```

### **Servicios (Implementaciones)**

```
src/ApiDePapas.Application/Services/
├── KeycloakTokenService.cs   # Obtiene y cachea tokens
├── ComprasApiClient.cs       # Implementa IComprasApiClient
└── StockApiClient.cs         # Implementa IStockApiClient
```

### **Controlador de Ejemplo**

```
src/ApiDePapas/Controllers/
└── IntegracionController.cs  # Ejemplos de uso de los clientes
```

---

## ⚙️ Configuración

### **appsettings.json**

Ya está configurado con:

```json
{
  "Keycloak": {
    "TokenEndpoint": "http://localhost:8080/realms/ds-2025-realm/protocol/openid-connect/token",
    "ClientId": "grupo-06",
    "ClientSecret": "8dc00e75-ccea-4d1a-be3d-b586733e256c"
  },
  "ExternalApis": {
    "ComprasApi": {
      "BaseUrl": "http://localhost:8081",
      "Timeout": 30
    },
    "StockApi": {
      "BaseUrl": "http://localhost:8082",
      "Timeout": 30
    }
  }
}
```

**⚠️ IMPORTANTE - Actualizar URLs:**

Las URLs `http://localhost:8081` y `http://localhost:8082` son **placeholders**. Debes cambiarlas por las URLs reales de las APIs de Compras y Stock:

```json
"ExternalApis": {
  "ComprasApi": {
    "BaseUrl": "http://grupo01-compras.example.com",  // ← URL real de la API de Compras
    "Timeout": 30
  },
  "StockApi": {
    "BaseUrl": "http://grupo02-stock.example.com",    // ← URL real de la API de Stock
    "Timeout": 30
  }
}
```

---

## 🔧 Cómo Usar en tu Código

### **1. Inyectar los clientes en tus servicios**

```csharp
public class ShippingService : IShippingService
{
    private readonly IComprasApiClient _comprasClient;
    private readonly IStockApiClient _stockClient;

    public ShippingService(
        IComprasApiClient comprasClient,
        IStockApiClient stockClient)
    {
        _comprasClient = comprasClient;
        _stockClient = stockClient;
    }

    public async Task<Shipping> CreateShippingAsync(CreateShippingRequest request)
    {
        // Verificar stock antes de crear el envío
        var stock = await _stockClient.GetStockAsync(request.producto_id);
        
        if (stock == null || stock.cantidad_disponible < request.cantidad)
        {
            throw new InvalidOperationException("Stock insuficiente");
        }

        // Reservar stock
        await _stockClient.ReservarStockAsync(new ReservaStockRequest
        {
            producto_id = request.producto_id,
            cantidad = request.cantidad,
            motivo = $"Envío {request.shipping_id}"
        });

        // ... resto de la lógica de creación de envío
    }
}
```

### **2. Usar en controladores**

```csharp
[ApiController]
[Route("api/shipping")]
[Authorize]
public class ShippingController : ControllerBase
{
    private readonly IComprasApiClient _comprasClient;
    private readonly IStockApiClient _stockClient;

    public ShippingController(
        IComprasApiClient comprasClient,
        IStockApiClient stockClient)
    {
        _comprasClient = comprasClient;
        _stockClient = stockClient;
    }

    [HttpPost]
    public async Task<ActionResult> CreateShipping([FromBody] CreateRequest request)
    {
        // Obtener información de la orden de compra
        var orden = await _comprasClient.GetOrdenCompraAsync(request.orden_id);
        
        if (orden == null)
        {
            return NotFound("Orden no encontrada");
        }

        // Verificar stock de todos los productos
        foreach (var item in orden.items)
        {
            var disponibilidad = await _stockClient.VerificarDisponibilidadAsync(
                item.producto_id, 
                item.cantidad);

            if (!disponibilidad.disponible)
            {
                return BadRequest($"Stock insuficiente para producto {item.producto_id}");
            }
        }

        // Crear el envío
        // ...
    }
}
```

---

## 📡 Endpoints de Ejemplo Disponibles

El controlador `IntegracionController` incluye endpoints de ejemplo:

### **GET /api/integracion/orden/{ordenId}**
Obtiene una orden de compra por ID.

**Ejemplo:**
```bash
curl -X GET "http://localhost:5000/api/integracion/orden/123" \
  -H "Authorization: Bearer <TU_TOKEN>"
```

### **GET /api/integracion/stock/{productoId}**
Obtiene el stock de un producto.

**Ejemplo:**
```bash
curl -X GET "http://localhost:5000/api/integracion/stock/456" \
  -H "Authorization: Bearer <TU_TOKEN>"
```

### **POST /api/integracion/reservar-stock**
Verifica disponibilidad y crea una reserva de stock.

**Ejemplo:**
```bash
curl -X POST "http://localhost:5000/api/integracion/reservar-stock" \
  -H "Authorization: Bearer <TU_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "producto_id": 456,
    "cantidad": 10,
    "motivo": "Envío #789"
  }'
```

### **GET /api/integracion/productos-con-stock**
Obtiene productos de la API de Compras con su stock de la API de Stock.

**Ejemplo:**
```bash
curl -X GET "http://localhost:5000/api/integracion/productos-con-stock" \
  -H "Authorization: Bearer <TU_TOKEN>"
```

### **POST /api/integracion/procesar-orden**
Workflow completo: crea orden y reserva stock.

**Ejemplo:**
```bash
curl -X POST "http://localhost:5000/api/integracion/procesar-orden" \
  -H "Authorization: Bearer <TU_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "usuario_id": 1,
    "items": [
      { "producto_id": 101, "cantidad": 2 },
      { "producto_id": 102, "cantidad": 5 }
    ]
  }'
```

---

## 🧪 Cómo Probar la Integración

### **Escenario 1: Probar sin APIs reales (Mock)**

Si las APIs de Compras y Stock aún no están disponibles, puedes crear un mock server:

**Opción A: Usar una herramienta como json-server**

```bash
# Instalar json-server globalmente
npm install -g json-server

# Crear un archivo db.json con datos de prueba
echo '{
  "ordenes": [
    {
      "id": 1,
      "usuario_id": 1,
      "estado": "pendiente",
      "fecha_creacion": "2025-01-01T10:00:00Z",
      "items": [
        { "producto_id": 101, "cantidad": 2, "precio_unitario": 100 }
      ],
      "total": 200
    }
  ],
  "productos": [
    {
      "id": 101,
      "nombre": "Producto Test",
      "descripcion": "Descripción de prueba",
      "precio": 100,
      "categoria_id": 1,
      "activo": true
    }
  ],
  "stock": [
    {
      "producto_id": 101,
      "cantidad_disponible": 50,
      "cantidad_reservada": 10,
      "ultima_actualizacion": "2025-01-01T10:00:00Z"
    }
  ]
}' > db.json

# Ejecutar el servidor en puerto 8081 (Compras)
json-server --watch db.json --port 8081

# En otra terminal, ejecutar en puerto 8082 (Stock)
json-server --watch db.json --port 8082
```

**Opción B: Usar Postman Mock Server**

1. Crear un Mock Server en Postman
2. Configurar las URLs en `appsettings.json`

---

### **Escenario 2: Probar con APIs reales**

**Paso 1: Obtener URLs de las APIs**

Contacta con los grupos de Compras y Stock para obtener sus URLs y documentación:

```
Grupo 01 (Compras): http://grupo01.example.com
Grupo 02 (Stock):   http://grupo02.example.com
```

**Paso 2: Actualizar appsettings.json**

```json
"ExternalApis": {
  "ComprasApi": {
    "BaseUrl": "http://grupo01.example.com"
  },
  "StockApi": {
    "BaseUrl": "http://grupo02.example.com"
  }
}
```

**Paso 3: Verificar autenticación de esas APIs**

Asegúrate de que las APIs de Compras y Stock:
- Estén configuradas con Keycloak
- Acepten tokens del realm `ds-2025-realm`
- Validen el token del `grupo-06`

**Paso 4: Probar endpoints**

```bash
# 1. Obtener tu token
TOKEN=$(curl -s -X POST "http://localhost:8080/realms/ds-2025-realm/protocol/openid-connect/token" \
  -d "grant_type=client_credentials" \
  -d "client_id=grupo-06" \
  -d "client_secret=8dc00e75-ccea-4d1a-be3d-b586733e256c" | jq -r '.access_token')

# 2. Probar endpoint de integración
curl -X GET "http://localhost:5000/api/integracion/productos-con-stock" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🔍 Logs y Debugging

### **Logs del KeycloakTokenService**

Cuando tu API solicita un token, verás logs como:

```
[12:00:00 INF] Solicitando nuevo token de Keycloak
[12:00:01 INF] Token obtenido exitosamente, expira en 300 segundos
```

Cuando usa el token en caché:

```
[12:01:00 DBG] Usando token en cache, expira en 240 segundos
```

### **Logs de los clientes HTTP**

```
[12:00:05 INF] Obteniendo orden de compra 123
[12:00:06 INF] Orden 123 creada exitosamente

[12:00:10 INF] Obteniendo stock del producto 456
[12:00:11 INF] Reservando stock: Producto 456, Cantidad 10
[12:00:12 INF] Reserva 789 creada exitosamente
```

### **Logs de errores comunes**

**Token inválido o expirado:**
```
[12:00:00 ERR] Error obteniendo token de Keycloak: Unauthorized
```
→ Verificar `ClientId` y `ClientSecret` en appsettings.json

**API no disponible:**
```
[12:00:00 ERR] Error obteniendo orden 123: No connection could be made
```
→ Verificar que la API de Compras esté corriendo en la URL configurada

**Timeout:**
```
[12:00:30 ERR] Error obteniendo stock: The operation was canceled
```
→ La API de Stock no respondió en 30 segundos. Aumentar `Timeout` en appsettings.json

---

## 📚 APIs Disponibles en los Clientes

### **IComprasApiClient**

```csharp
// Obtener orden por ID
OrdenCompraResponse? orden = await _comprasClient.GetOrdenCompraAsync(123);

// Obtener órdenes de un usuario
List<OrdenCompraResponse> ordenes = await _comprasClient.GetOrdenesByUsuarioAsync(1);

// Crear nueva orden
OrdenCompraResponse nuevaOrden = await _comprasClient.CrearOrdenCompraAsync(new CrearOrdenCompraRequest
{
    usuario_id = 1,
    items = new List<ItemCompraRequest>
    {
        new() { producto_id = 101, cantidad = 2 },
        new() { producto_id = 102, cantidad = 5 }
    }
});

// Obtener producto por ID
ProductoResponse? producto = await _comprasClient.GetProductoAsync(101);

// Obtener todos los productos
List<ProductoResponse> productos = await _comprasClient.GetProductosAsync();
```

### **IStockApiClient**

```csharp
// Obtener stock de un producto
StockResponse? stock = await _stockClient.GetStockAsync(101);

// Verificar disponibilidad
StockDisponibleResponse disponibilidad = await _stockClient.VerificarDisponibilidadAsync(101, 10);
if (disponibilidad.disponible)
{
    // Hay stock suficiente
}

// Reservar stock
ReservaStockResponse reserva = await _stockClient.ReservarStockAsync(new ReservaStockRequest
{
    producto_id = 101,
    cantidad = 10,
    motivo = "Envío #123"
});

// Actualizar stock
StockResponse nuevoStock = await _stockClient.ActualizarStockAsync(new ActualizarStockRequest
{
    producto_id = 101,
    cantidad = 20,
    operacion = "incrementar" // o "decrementar"
});

// Liberar reserva
bool liberado = await _stockClient.LiberarReservaAsync(789);
```

---

## 🚨 Manejo de Errores

### **Errores de red**

```csharp
try
{
    var stock = await _stockClient.GetStockAsync(productoId);
}
catch (HttpRequestException ex)
{
    // API no disponible, timeout, etc.
    _logger.LogError(ex, "Error de red al comunicarse con API de Stock");
    return StatusCode(503, new { message = "Servicio temporalmente no disponible" });
}
```

### **Errores de autenticación**

```csharp
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    _logger.LogError("Token inválido o expirado");
    // El KeycloakTokenService renovará automáticamente el token en el próximo intento
}
```

### **Recursos no encontrados**

```csharp
var producto = await _comprasClient.GetProductoAsync(productoId);
if (producto == null)
{
    return NotFound($"Producto {productoId} no encontrado");
}
```

---

## 🔒 Seguridad

### **Protección del Client Secret**

**❌ NO HACER:**
- Commitear el `client_secret` en Git
- Exponer el secret en logs
- Compartir el secret públicamente

**✅ HACER:**
- Usar variables de entorno en producción:
  ```bash
  export KEYCLOAK_CLIENT_SECRET="8dc00e75-ccea-4d1a-be3d-b586733e256c"
  ```
- Configurar en Azure App Settings, AWS Secrets Manager, etc.
- Actualizar código para leer de variables de entorno:
  ```csharp
  var clientSecret = builder.Configuration["KEYCLOAK_CLIENT_SECRET"] 
      ?? builder.Configuration["Keycloak:ClientSecret"];
  ```

### **Renovación automática de tokens**

El `KeycloakTokenService` maneja automáticamente:
- ✅ Caché de tokens en memoria
- ✅ Renovación 1 minuto antes de expirar
- ✅ Thread-safe (usa `SemaphoreSlim`)
- ✅ Evita múltiples solicitudes simultáneas

---

## 📊 Diagrama de Flujo: Workflow Completo

```
Usuario hace POST /api/shipping
         │
         ▼
  ┌──────────────────┐
  │ Tu API Logística │
  └────────┬─────────┘
           │
           ├─1─► Obtener token de Keycloak (automático)
           │     └─► KeycloakTokenService.GetAccessTokenAsync()
           │
           ├─2─► Obtener orden de compra
           │     └─► _comprasClient.GetOrdenCompraAsync(orderId)
           │           │
           │           └─► GET http://grupo01.com/api/ordenes/123
           │               Headers: Authorization: Bearer <token>
           │
           ├─3─► Verificar stock de cada producto
           │     └─► _stockClient.VerificarDisponibilidadAsync(...)
           │           │
           │           └─► GET http://grupo02.com/api/stock/456/disponibilidad
           │               Headers: Authorization: Bearer <token>
           │
           ├─4─► Reservar stock
           │     └─► _stockClient.ReservarStockAsync(...)
           │           │
           │           └─► POST http://grupo02.com/api/stock/reservas
           │               Headers: Authorization: Bearer <token>
           │               Body: { producto_id, cantidad, motivo }
           │
           └─5─► Crear envío en tu DB
                 └─► Retornar respuesta al usuario
```

---

## ✅ Checklist de Integración

### **Configuración**
- [ ] Actualizar `ExternalApis:ComprasApi:BaseUrl` con URL real
- [ ] Actualizar `ExternalApis:StockApi:BaseUrl` con URL real
- [ ] Verificar que `Keycloak:ClientId` sea `grupo-06`
- [ ] Verificar que `Keycloak:ClientSecret` sea correcto
- [ ] Proteger `ClientSecret` (variables de entorno en producción)

### **Coordinación con otros grupos**
- [ ] Obtener URL de la API de Compras (Grupo 01, 04, 07, 10 o 13)
- [ ] Obtener URL de la API de Stock (Grupo 02, 05, 08 o 11)
- [ ] Obtener documentación de endpoints de ambas APIs
- [ ] Verificar que ambas APIs acepten tokens de Keycloak
- [ ] Probar autenticación con tus credenciales (`grupo-06`)

### **Pruebas**
- [ ] Probar obtención de token (KeycloakTokenService)
- [ ] Probar endpoints de IntegracionController
- [ ] Verificar logs de autenticación
- [ ] Probar manejo de errores (API no disponible, token inválido)
- [ ] Probar workflow completo (crear orden + reservar stock)

### **Documentación**
- [ ] Documentar endpoints de tu API que usan integración
- [ ] Compartir ejemplos de uso con otros grupos
- [ ] Documentar errores comunes y soluciones

---

## 🎯 Próximos Pasos

1. **Contactar a otros grupos** para obtener URLs de sus APIs
2. **Probar la integración** con datos reales
3. **Implementar workflows específicos** en tus servicios de negocio:
   - Al crear un envío, verificar stock
   - Al cancelar un envío, liberar reservas de stock
   - Obtener información de productos para cálculos de costo
4. **Agregar retry policies** con Polly para mayor resiliencia
5. **Implementar circuit breakers** para evitar cascadas de fallos
6. **Monitorear métricas** de llamadas a APIs externas

---

## 📞 Soporte

Si tienes problemas con la integración:

1. **Revisar logs** de tu API (Console Output)
2. **Revisar logs** de Keycloak (Docker logs)
3. **Verificar conectividad** con las APIs externas (ping, curl)
4. **Verificar tokens** en jwt.io
5. **Contactar a los grupos** de Compras/Stock si sus APIs no responden

---

¡Tu API está lista para integrarse con las APIs de Compras y Stock! 🚀
