using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ApiDePapas.Application.DTOs.External;
using ApiDePapas.Application.Interfaces;

namespace ApiDePapas.Application.Services;

/// <summary>
/// Cliente HTTP tipado para la API de Compras
/// </summary>
public class ComprasApiClient : IComprasApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IKeycloakTokenService _tokenService;
    private readonly ILogger<ComprasApiClient> _logger;

    public ComprasApiClient(
        HttpClient httpClient,
        IKeycloakTokenService tokenService,
        ILogger<ComprasApiClient> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<OrdenCompraResponse?> GetOrdenCompraAsync(int ordenId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Obteniendo orden de compra {OrdenId}", ordenId);

            var response = await _httpClient.GetAsync($"/api/ordenes/{ordenId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Orden de compra {OrdenId} no encontrada", ordenId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrdenCompraResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo orden de compra {OrdenId}", ordenId);
            throw;
        }
    }

    public async Task<List<OrdenCompraResponse>> GetOrdenesByUsuarioAsync(int usuarioId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Obteniendo órdenes del usuario {UsuarioId}", usuarioId);

            var response = await _httpClient.GetAsync($"/api/ordenes?usuario_id={usuarioId}");
            response.EnsureSuccessStatusCode();

            var ordenes = await response.Content.ReadFromJsonAsync<List<OrdenCompraResponse>>();
            return ordenes ?? new List<OrdenCompraResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo órdenes del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    public async Task<OrdenCompraResponse> CrearOrdenCompraAsync(CrearOrdenCompraRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Creando orden de compra para usuario {UsuarioId}", request.usuario_id);

            var response = await _httpClient.PostAsJsonAsync("/api/ordenes", request);
            response.EnsureSuccessStatusCode();

            var orden = await response.Content.ReadFromJsonAsync<OrdenCompraResponse>();
            if (orden == null)
            {
                throw new InvalidOperationException("Respuesta inválida al crear orden de compra");
            }

            _logger.LogInformation("Orden de compra {OrdenId} creada exitosamente", orden.id);
            return orden;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando orden de compra");
            throw;
        }
    }

    public async Task<ProductoResponse?> GetProductoAsync(int productoId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Obteniendo producto {ProductoId}", productoId);

            var response = await _httpClient.GetAsync($"/api/productos/{productoId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Producto {ProductoId} no encontrado", productoId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductoResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo producto {ProductoId}", productoId);
            throw;
        }
    }

    public async Task<List<ProductoResponse>> GetProductosAsync()
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Obteniendo todos los productos");

            var response = await _httpClient.GetAsync("/api/productos");
            response.EnsureSuccessStatusCode();

            var productos = await response.Content.ReadFromJsonAsync<List<ProductoResponse>>();
            return productos ?? new List<ProductoResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo productos");
            throw;
        }
    }
}
