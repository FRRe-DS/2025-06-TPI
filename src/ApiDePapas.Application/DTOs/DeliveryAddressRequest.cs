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
        var address = new Address();
        address.street = street;
        address.number = number;
        address.postal_code = postal_code;
        address.locality_name = locality_name;

        return address;
    }
}