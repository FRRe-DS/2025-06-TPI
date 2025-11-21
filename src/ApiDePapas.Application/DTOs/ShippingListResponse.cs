using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Application.DTOs;

// Modelo para la respuesta de la lista paginada de envíos.
public record ShippingListResponse(
    [property: JsonPropertyName("shipments")]
    [Required]
    List<ShipmentSummary> Shipments,

    [property: JsonPropertyName("pagination")]
    [Required]
    PaginationData Pagination
);