using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ApiDePapas.Application.DTOs.External;
using ApiDePapas.Application.Interfaces;

namespace ApiDePapas.Application.Services;

/// <summary>
/// Servicio para obtener y cachear tokens de Keycloak
/// </summary>
public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakTokenService> _logger;
    
    private string? _cachedToken;
    private DateTime _tokenExpiresAt = DateTime.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public KeycloakTokenService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<KeycloakTokenService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        // Si el token está en cache y no ha expirado, retornarlo
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiresAt.AddMinutes(-1))
        {
            _logger.LogDebug("Usando token en cache, expira en {ExpiresIn} segundos", 
                (_tokenExpiresAt - DateTime.UtcNow).TotalSeconds);
            return _cachedToken;
        }

        // Usar semáforo para evitar múltiples solicitudes simultáneas
        await _semaphore.WaitAsync();
        try
        {
            // Verificar nuevamente después de obtener el lock
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiresAt.AddMinutes(-1))
            {
                return _cachedToken;
            }

            _logger.LogInformation("Solicitando nuevo token de Keycloak");

            var tokenEndpoint = _configuration["Keycloak:TokenEndpoint"] 
                ?? throw new InvalidOperationException("Keycloak:TokenEndpoint no configurado");
            var clientId = _configuration["Keycloak:ClientId"] 
                ?? throw new InvalidOperationException("Keycloak:ClientId no configurado");
            var clientSecret = _configuration["Keycloak:ClientSecret"] 
                ?? throw new InvalidOperationException("Keycloak:ClientSecret no configurado");

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var response = await _httpClient.PostAsync(
                tokenEndpoint,
                new FormUrlEncodedContent(requestBody));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error obteniendo token de Keycloak: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                throw new HttpRequestException(
                    $"Error obteniendo token de Keycloak: {response.StatusCode}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
            {
                throw new InvalidOperationException("Respuesta de token inválida de Keycloak");
            }

            _cachedToken = tokenResponse.access_token;
            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);

            _logger.LogInformation("Token obtenido exitosamente, expira en {ExpiresIn} segundos", 
                tokenResponse.expires_in);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
