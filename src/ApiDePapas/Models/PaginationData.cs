using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    // Contiene la metadata para las respuestas paginadas.
    public record PaginationData(
        [property: JsonPropertyName("current_page")]
        [property: Required]
        int? CurrentPage,

        [property: JsonPropertyName("total_pages")]
        [property: Required]
        int? TotalPages,

        [property: JsonPropertyName("total_items")]
        [property: Required]
        int? TotalItems,

        [property: JsonPropertyName("items_per_page")]
        [property: Required]
        int? ItemsPerPage
    );
}