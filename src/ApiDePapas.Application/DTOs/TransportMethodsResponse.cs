using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Application.DTOs;

public class TransportMethodsResponse
{
    [Required]
    public List<TransportMethods> transport_methods { get; set; } = new List<TransportMethods>();
}