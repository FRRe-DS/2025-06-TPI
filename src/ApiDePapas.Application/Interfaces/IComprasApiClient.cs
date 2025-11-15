using ApiDePapas.Application.DTOs.External;

namespace ApiDePapas.Application.Interfaces;

/// <summary>
/// Cliente para interactuar con la API de Compras
/// </summary>
public interface IComprasApiClient
{
    /// <summary>
    /// Obtiene una orden de compra por ID
    /// </summary>
    Task<OrdenCompraResponse?> GetOrdenCompraAsync(int ordenId);

    /// <summary>
    /// Obtiene todas las órdenes de compra de un usuario
    /// </summary>
    Task<List<OrdenCompraResponse>> GetOrdenesByUsuarioAsync(int usuarioId);

    /// <summary>
    /// Crea una nueva orden de compra
    /// </summary>
    Task<OrdenCompraResponse> CrearOrdenCompraAsync(CrearOrdenCompraRequest request);

    /// <summary>
    /// Obtiene información de un producto
    /// </summary>
    Task<ProductoResponse?> GetProductoAsync(int productoId);

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    Task<List<ProductoResponse>> GetProductosAsync();
}
