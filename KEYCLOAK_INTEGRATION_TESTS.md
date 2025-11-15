# 🔐 Pruebas de Integración Keycloak - API Logística Grupo 06

## ✅ Cambios Implementados

La API ahora tiene autenticación JWT integrada con Keycloak:

1. ✅ Instalado `Microsoft.AspNetCore.Authentication.JwtBearer`
2. ✅ Configurado `appsettings.json` con parámetros de Keycloak
3. ✅ Actualizado `Program.cs` con middleware de autenticación/autorización
4. ✅ Protegidos todos los controladores con `[Authorize]`

---

## 🚀 Paso 1: Iniciar Keycloak

```powershell
cd c:\Users\Tobias\Desktop\DDS\2025-06-TPI\src\keycloak
docker-compose up -d
```

Verificar que esté corriendo:
```powershell
docker ps
```

Acceder a Keycloak Admin Console:
- **URL**: http://localhost:8080
- **Usuario**: admin
- **Password**: ds2025

---

## 🚀 Paso 2: Iniciar tu API

```powershell
cd c:\Users\Tobias\Desktop\DDS\2025-06-TPI\src\ApiDePapas
dotnet run
```

La API debería iniciar en `http://localhost:5000` (o el puerto configurado).

---

## 🧪 Paso 3: Probar Autenticación

### **Prueba 1: Llamar sin token (debe fallar con 401)**

```powershell
curl http://localhost:5000/api/shipping
```

**Resultado esperado**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

### **Prueba 2: Obtener token JWT del Grupo 06**

```powershell
$response = Invoke-RestMethod -Uri "http://localhost:8080/realms/ds-2025-realm/protocol/openid-connect/token" `
  -Method POST `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
    grant_type = "client_credentials"
    client_id = "grupo-06"
    client_secret = "8dc00e75-ccea-4d1a-be3d-b586733e256c"
  }

$token = $response.access_token
Write-Host "Token obtenido: $token"
```

**Resultado esperado**:
```
Token obtenido: eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjEyMyJ9...
```

---

### **Prueba 3: Llamar con token (debe funcionar con 200)**

```powershell
# Usando el token obtenido en la prueba anterior
Invoke-RestMethod -Uri "http://localhost:5000/api/shipping" `
  -Method GET `
  -Headers @{
    "Authorization" = "Bearer $token"
    "Accept" = "application/json"
  }
```

**Resultado esperado**: 200 OK con datos de shipments.

---

### **Prueba 4: Verificar token en jwt.io**

1. Copiar el token
2. Ir a https://jwt.io
3. Pegar el token en el campo "Encoded"
4. Verificar los claims:
   - `iss`: `http://localhost:8080/realms/ds-2025-realm`
   - `aud`: `account`
   - `azp`: `grupo-06`
   - `realm_access.roles`: debe incluir `logistica-be`
   - `scope`: debe incluir `envios:read envios:write`

---

## 🧪 Probar con tokens de otros grupos

### **Obtener token del Grupo 01 (Compras)**

```powershell
$response01 = Invoke-RestMethod -Uri "http://localhost:8080/realms/ds-2025-realm/protocol/openid-connect/token" `
  -Method POST `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
    grant_type = "client_credentials"
    client_id = "grupo-01"
    client_secret = "68230b9a-f540-4e16-9f56-19180f303676"
  }

$token01 = $response01.access_token

# Llamar a tu API con el token del grupo-01
Invoke-RestMethod -Uri "http://localhost:5000/api/shipping" `
  -Method GET `
  -Headers @{
    "Authorization" = "Bearer $token01"
    "Accept" = "application/json"
  }
```

**Resultado esperado**: 200 OK (el token es válido, solo valida issuer y audience).

---

## 📝 Logs de la API

Revisa los logs de tu API en la consola. Deberías ver:

**Cuando el token es válido**:
```
Token validated for: service-account-grupo-06
```

**Cuando falla la autenticación**:
```
Authentication failed: No authorization header found
```
o
```
Authentication failed: IDX10223: Lifetime validation failed. The token is expired.
```

---

## 🔧 Troubleshooting

### **Error: "Unable to obtain configuration from http://localhost:8080/..."**

**Causa**: Keycloak no está corriendo o no es accesible.

**Solución**:
```powershell
cd c:\Users\Tobias\Desktop\DDS\2025-06-TPI\src\keycloak
docker-compose up -d
docker ps  # Verificar que keycloak esté corriendo
```

---

### **Error: "IDX10205: Issuer validation failed"**

**Causa**: El `Authority` en `appsettings.json` no coincide con el issuer del token.

**Solución**: Verificar que `appsettings.json` tenga:
```json
"Keycloak": {
  "Authority": "http://localhost:8080/realms/ds-2025-realm"
}
```

---

### **Error: "IDX10214: Audience validation failed"**

**Causa**: El claim `aud` del token no coincide con `Keycloak:Audience`.

**Solución**: Verificar que `appsettings.json` tenga:
```json
"Keycloak": {
  "Audience": "account"
}
```

---

## 🎯 Próximos Pasos (Opcional)

### **1. Validar roles específicos**

Cambiar las políticas en `Program.cs` para usar roles del realm:

```csharp
options.AddPolicy("LogisticaBackend", policy =>
    policy.RequireAssertion(context =>
        context.User.Claims.Any(c => 
            c.Type == "realm_access" && 
            c.Value.Contains("logistica-be")
        )
    ));
```

### **2. Configurar Audience específico**

En Keycloak Admin Console:
1. **Clients** → **grupo-06** → **Client Scopes** → **grupo-06-dedicated**
2. **Add mapper** → **Audience**
   - Name: `audience-mapper`
   - Included Client Audience: `grupo-06-api`
   - Add to access token: ON
3. Actualizar `appsettings.json`:
   ```json
   "Keycloak": {
     "Audience": "grupo-06-api"
   }
   ```

### **3. Proteger endpoints específicos con políticas**

```csharp
[HttpPost]
[Authorize(Policy = "EnviosWrite")]
public async Task<ActionResult> CreateShipment(...)
{
    // Solo clientes con scope "envios:write"
}
```

---

## 📋 Credenciales de Clientes (para pruebas)

| Grupo | Client ID | Client Secret | Rol |
|-------|-----------|---------------|-----|
| 01 | grupo-01 | 68230b9a-f540-4e16-9f56-19180f303676 | compras-be |
| 02 | grupo-02 | 58536bf8-8501-41c9-b411-786b6d654c25 | stock-be |
| 03 | grupo-03 | 21cd6616-6571-4ee7-be29-0f781f77c74e | logistica-be |
| 04 | grupo-04 | 6be1bec1-9472-499f-ab37-883d78f57829 | compras-be |
| 05 | grupo-05 | 9e676dd4-2790-4191-9f1f-06c6c6fd71e5 | stock-be |
| **06** | **grupo-06** | **8dc00e75-ccea-4d1a-be3d-b586733e256c** | **logistica-be** |
| 08 | grupo-08 | 248f42b5-7007-47d1-a94e-e8941f352f6f | stock-be |
| 09 | grupo-09 | 3a0ca5f2-00cc-4016-b7a2-7aad04eda3af | logistica-be |
| 10 | grupo-10 | 66ff9787-4fa5-46b3-b546-4ccbe604d233 | compras-be |
| 11 | grupo-11 | ef7f0900-8de5-46c0-b813-ce76d61e0158 | stock-be |
| 12 | grupo-12 | ebbd9f47-b6ff-4efe-9fdd-71651a87139c | logistica-be |
| 13 | grupo-13 | 404249de-18ba-403c-b45c-d82c446e2a2a | compras-be |

---

## ✅ Checklist Final

- [ ] Keycloak corriendo en http://localhost:8080
- [ ] API corriendo (dotnet run)
- [ ] Llamada sin token → 401 Unauthorized
- [ ] Obtener token con grupo-06 → Token válido
- [ ] Llamada con token → 200 OK
- [ ] Token decodificado en jwt.io con claims correctos
- [ ] Logs de la API mostrando "Token validated for: service-account-grupo-06"

¡Tu API está lista para producción con autenticación JWT! 🚀
