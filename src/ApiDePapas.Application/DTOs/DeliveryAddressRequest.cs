using System.Collections.Generic;
using ApiDePapas.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Application.DTOs
{
    // Define el DTO de la Dirección de ENTRADA
    public record DeliveryAddressRequest(
        [property: JsonPropertyName("street")]
        [Required]
        string street = "",

        [property: JsonPropertyName("number")]
        [Required]
        int number = 0,

        [property: JsonPropertyName("postal_code")]
        [Required]
        string postal_code = "",

        //sinonimo de city
        [property: JsonPropertyName("locality_name")]
        [Required]
        string locality_name = "",

        
        // AGREGADO: Para cumplir con el esquema, aunque internamente usemos solo AR
        // los agrego por la api pero frontend puede omitirlos por completo y no pasará nada malo. Son opcionales porque la BDD con localidad los encuentra
        [property: JsonPropertyName("state")]
        string? state = null,

        [property: JsonPropertyName("country")]
        string? country = null
    );
}