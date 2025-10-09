using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ShippingCostResponse(
        [property: JsonPropertyName("currency")]
        [property: Required]
        string Currency,

        [property: JsonPropertyName("total_cost")]
        [property: Required]
        double? TotalCost,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        TransportType? TransportType,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductOutput> Products
    );
}