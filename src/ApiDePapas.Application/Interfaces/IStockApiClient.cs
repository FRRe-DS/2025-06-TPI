using ApiDePapas.Application.DTOs.External;

namespace ApiDePapas.Application.Interfaces;

/// <summary>
/// Cliente para interactuar con la API de Stock
/// </summary>
public interface IStockApiClient
{
    /// <summary>
    /// Obtiene el stock disponible de un producto
    /// </summary>
    Task<StockResponse?> GetStockAsync(int productoId);

    /// <summary>
    /// Verifica si hay stock disponible para una cantidad específica
    /// </summary>
    Task<StockDisponibleResponse> VerificarDisponibilidadAsync(int productoId, int cantidad);

    /// <summary>
    /// Crea una reserva de stock
    /// </summary>
    Task<ReservaStockResponse> ReservarStockAsync(ReservaStockRequest request);

    /// <summary>
    /// Actualiza el stock de un producto
    /// </summary>
    Task<StockResponse> ActualizarStockAsync(ActualizarStockRequest request);

    /// <summary>
    /// Libera una reserva de stock
    /// </summary>
    Task<bool> LiberarReservaAsync(int reservaId);
}
