namespace ApiDePapas.Application.DTOs.External;

/// <summary>
/// Respuesta del token de Keycloak
/// </summary>
public record KeycloakTokenResponse
{
    public string access_token { get; init; } = string.Empty;
    public int expires_in { get; init; }
    public int refresh_expires_in { get; init; }
    public string token_type { get; init; } = string.Empty;
    public string scope { get; init; } = string.Empty;
}
