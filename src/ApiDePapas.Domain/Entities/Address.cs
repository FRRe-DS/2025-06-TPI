using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Domain.Entities;

public class Address
{
    [Required]
    [Key]
    public int address_id { get; set; }

    [property: JsonPropertyName("street")]
    [Required]
    public string street { get; set; } = string.Empty;

    [property: JsonPropertyName("number")]
    public int number { get; set; } = 0; 

    [property: JsonPropertyName("postal_code")]
    [Required]
    public string postal_code { get; set; } = string.Empty;

    [property: JsonPropertyName("locality_name")]
    [Required]
    public string locality_name { get; set; } = string.Empty;

    public Locality Locality { get; set; } = null!;
    
    public ICollection<ShippingDetail> DeliveredShippings { get; set; } = new List<ShippingDetail>();
}