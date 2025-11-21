using ApiDePapas.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace ApiDePapas.Infrastructure.Auth;

/**
    * Servicio para obtener un Access Token de Keycloak
    * usando Client Credentials.
    */
public class KeycloakTokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private string _cachedToken;
    private DateTime _tokenExpiresAt;

    public KeycloakTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _tokenExpiresAt = DateTime.UtcNow;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        // Revisa si el token cacheado es válido (con un margen de 30s)
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiresAt > DateTime.UtcNow.AddSeconds(30))
        {
            return _cachedToken;
        }

        // Si no, pide uno nuevo
        var client = _httpClientFactory.CreateClient("KeycloakClient");
        
        var tokenEndpoint = _configuration["Keycloak:TokenEndpoint"];
        var clientId = _configuration["Keycloak:ClientId"];
        var clientSecret = _configuration["Keycloak:ClientSecret"];

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        var response = await client.PostAsync(tokenEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("No se pudo obtener el token de Keycloak.");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseString);
        
        _cachedToken = jsonDoc.RootElement.GetProperty("access_token").GetString();
        var expiresIn = jsonDoc.RootElement.GetProperty("expires_in").GetInt32();
        _tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        return _cachedToken;
    }
}