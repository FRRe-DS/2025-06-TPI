using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Queries;

namespace ApiDePapas.Application.DTOs;

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

    [property: JsonPropertyName("locality_name")]
    [Required]
    string locality_name = ""
)
{
    public AddressQuery ToQuery() =>  new AddressQuery(street, number, postal_code, locality_name);
    public Address ToAddress()
    {
        return new Address()
        {
            street = street,
            number = number,
            postal_code = postal_code,
            locality_name = locality_name    
        };
    }
}