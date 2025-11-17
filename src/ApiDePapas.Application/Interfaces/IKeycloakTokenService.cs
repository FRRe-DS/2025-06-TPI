using ApiDePapas.Application.DTOs.External;

namespace ApiDePapas.Application.Interfaces;

/// <summary>
/// Servicio para obtener tokens de Keycloak
/// </summary>
public interface IKeycloakTokenService
{
    /// <summary>
    /// Obtiene un token JWT válido (cachea y renueva automáticamente)
    /// </summary>
    Task<string> GetAccessTokenAsync();
}
