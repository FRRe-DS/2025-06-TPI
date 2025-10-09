using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ShippingCostResponse(
        [property: JsonPropertyName("currency")]
        [property: Required]
        string currency,

        [property: JsonPropertyName("total_cost")]
        [property: Required]
        double? total_cost,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        TransportType? transport_type,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductOutput> products
    );
}