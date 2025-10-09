using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

//Se usa en ShippingDetails

namespace ApiDePapas.Models
{
    // Modelo de SALIDA (Detalle): Representa un producto dentro de un envío ya creado.
    public record ProductQty(
        [property: JsonPropertyName("product_id")]
        [property: Required]
        int? ProductId,

        [property: JsonPropertyName("quantity")]
        [property: Required]
        [property: Range(1, int.MaxValue)]
        int? Quantity
    );
}