using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Application.DTOs
{
    // Modelo para la respuesta de la lista paginada de envíos.
    public record ShippingListResponse(
        [property: JsonPropertyName("shipments")]
        [property: Required]
        List<ShipmentSummary> Shipments,

        [property: JsonPropertyName("pagination")]
        [property: Required]
        PaginationData Pagination
    );
}
