using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

//modelo interno que usamos nosotros, parte del ShippingCostResponse

namespace ApiDePapas.Models
{
    // Modelo de SALIDA (Cotización): Representa el costo de un producto en una cotización.
    public record ProductCost(
        [property: JsonPropertyName("id")]
        [property: Required]
        int? Id,

        [property: JsonPropertyName("cost")]
        [property: Required]
        double? Cost
    );
}