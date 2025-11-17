using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ApiDePapas.Application.DTOs.External;
using ApiDePapas.Application.Interfaces;

namespace ApiDePapas.Application.Services;

/// <summary>
/// Cliente HTTP tipado para la API de Stock
/// </summary>
public class StockApiClient : IStockApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IKeycloakTokenService _tokenService;
    private readonly ILogger<StockApiClient> _logger;

    public StockApiClient(
        HttpClient httpClient,
        IKeycloakTokenService tokenService,
        ILogger<StockApiClient> logger)
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

    public async Task<StockResponse?> GetStockAsync(int productoId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Obteniendo stock del producto {ProductoId}", productoId);

            var response = await _httpClient.GetAsync($"/api/stock/{productoId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Stock del producto {ProductoId} no encontrado", productoId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StockResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo stock del producto {ProductoId}", productoId);
            throw;
        }
    }

    public async Task<StockDisponibleResponse> VerificarDisponibilidadAsync(int productoId, int cantidad)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Verificando disponibilidad: Producto {ProductoId}, Cantidad {Cantidad}", 
                productoId, cantidad);

            var response = await _httpClient.GetAsync(
                $"/api/stock/{productoId}/disponibilidad?cantidad={cantidad}");
            
            response.EnsureSuccessStatusCode();
            
            var disponibilidad = await response.Content.ReadFromJsonAsync<StockDisponibleResponse>();
            if (disponibilidad == null)
            {
                throw new InvalidOperationException("Respuesta inválida al verificar disponibilidad");
            }

            return disponibilidad;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando disponibilidad del producto {ProductoId}", productoId);
            throw;
        }
    }

    public async Task<ReservaStockResponse> ReservarStockAsync(ReservaStockRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Reservando stock: Producto {ProductoId}, Cantidad {Cantidad}", 
                request.producto_id, request.cantidad);

            var response = await _httpClient.PostAsJsonAsync("/api/stock/reservas", request);
            response.EnsureSuccessStatusCode();

            var reserva = await response.Content.ReadFromJsonAsync<ReservaStockResponse>();
            if (reserva == null)
            {
                throw new InvalidOperationException("Respuesta inválida al reservar stock");
            }

            _logger.LogInformation("Reserva {ReservaId} creada exitosamente", reserva.reserva_id);
            return reserva;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reservando stock del producto {ProductoId}", request.producto_id);
            throw;
        }
    }

    public async Task<StockResponse> ActualizarStockAsync(ActualizarStockRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Actualizando stock: Producto {ProductoId}, Operación {Operacion}, Cantidad {Cantidad}", 
                request.producto_id, request.operacion, request.cantidad);

            var response = await _httpClient.PutAsJsonAsync($"/api/stock/{request.producto_id}", request);
            response.EnsureSuccessStatusCode();

            var stock = await response.Content.ReadFromJsonAsync<StockResponse>();
            if (stock == null)
            {
                throw new InvalidOperationException("Respuesta inválida al actualizar stock");
            }

            return stock;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando stock del producto {ProductoId}", request.producto_id);
            throw;
        }
    }

    public async Task<bool> LiberarReservaAsync(int reservaId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            _logger.LogInformation("Liberando reserva {ReservaId}", reservaId);

            var response = await _httpClient.DeleteAsync($"/api/stock/reservas/{reservaId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Reserva {ReservaId} no encontrada", reservaId);
                return false;
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Reserva {ReservaId} liberada exitosamente", reservaId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liberando reserva {ReservaId}", reservaId);
            throw;
        }
    }
}
