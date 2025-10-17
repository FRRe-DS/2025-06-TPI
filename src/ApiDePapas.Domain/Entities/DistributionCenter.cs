using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Domain.Entities
{
    public record DistributionCenter(

        [property: JsonPropertyName("distribution_center_id")]
        [Required]
        int distribution_center_id,

        [property: JsonPropertyName("address")]
        [Required]
        Address distribution_center_address
    );
}