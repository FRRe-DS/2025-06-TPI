using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ApiDePapas.Application.DTOs
{
    public class TransportMethodsResponde
    {
        [Required]
        public List<TransportMethods> transport_methods { get; set; } = new List<TransportMethods>();
    }
}
