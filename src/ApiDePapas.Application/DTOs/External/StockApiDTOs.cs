namespace ApiDePapas.Application.DTOs.External;

/// <summary>
/// DTOs para la API de Stock
/// </summary>
/// 
public record StockResponse
{
    public int producto_id { get; init; }
    public int cantidad_disponible { get; init; }
    public int cantidad_reservada { get; init; }
    public DateTime ultima_actualizacion { get; init; }
}

public record ReservaStockRequest
{
    public int producto_id { get; init; }
    public int cantidad { get; init; }
    public string motivo { get; init; } = string.Empty;
}

public record ReservaStockResponse
{
    public int reserva_id { get; init; }
    public int producto_id { get; init; }
    public int cantidad { get; init; }
    public string estado { get; init; } = string.Empty;
    public DateTime fecha_creacion { get; init; }
}

public record ActualizarStockRequest
{
    public int producto_id { get; init; }
    public int cantidad { get; init; }
    public string operacion { get; init; } = "incrementar"; // "incrementar" o "decrementar"
}

public record StockDisponibleResponse
{
    public bool disponible { get; init; }
    public int cantidad_disponible { get; init; }
}
