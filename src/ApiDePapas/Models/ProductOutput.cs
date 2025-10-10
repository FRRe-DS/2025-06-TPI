using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

//modelo interno que usamos nosotros, parte del ShippingCostResponse

namespace ApiDePapas.Models
{
    // Modelo de SALIDA (Cotización): Representa el costo de un producto en una cotización.
    public record ProductOutput (
        [property: JsonPropertyName("id")]
        [property: Required]
        int id,

        [property: JsonPropertyName("cost")]
        [property: Required]
        double cost
    );
}
