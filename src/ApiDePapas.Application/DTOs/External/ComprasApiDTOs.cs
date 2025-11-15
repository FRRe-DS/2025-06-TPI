namespace ApiDePapas.Application.DTOs.External;

/// <summary>
/// DTOs para la API de Compras
/// </summary>

public record OrdenCompraResponse
{
    public int id { get; init; }
    public int usuario_id { get; init; }
    public string estado { get; init; } = string.Empty;
    public DateTime fecha_creacion { get; init; }
    public List<ItemCompra> items { get; init; } = new();
    public decimal total { get; init; }
}

public record ItemCompra
{
    public int producto_id { get; init; }
    public int cantidad { get; init; }
    public decimal precio_unitario { get; init; }
}

public record ProductoResponse
{
    public int id { get; init; }
    public string nombre { get; init; } = string.Empty;
    public string descripcion { get; init; } = string.Empty;
    public decimal precio { get; init; }
    public int categoria_id { get; init; }
    public bool activo { get; init; }
}

public record CrearOrdenCompraRequest
{
    public int usuario_id { get; init; }
    public List<ItemCompraRequest> items { get; init; } = new();
}

public record ItemCompraRequest
{
    public int producto_id { get; init; }
    public int cantidad { get; init; }
}
