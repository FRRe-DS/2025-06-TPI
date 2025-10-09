using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    // Contiene la metadata para las respuestas paginadas.
    public record PaginationData(
        [property: JsonPropertyName("current_page")]
        [property: Required]
        int? current_page,

        [property: JsonPropertyName("total_pages")]
        [property: Required]
        int? total_pages,

        [property: JsonPropertyName("total_items")]
        [property: Required]
        int? total_items,

        [property: JsonPropertyName("items_per_page")]
        [property: Required]
        int? items_per_page
    );
}