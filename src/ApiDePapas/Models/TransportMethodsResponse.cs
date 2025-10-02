using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ApiDePapas.Models
{
    public class TransportMethodsResponde
    {
        [Required]
        public List<TransportMethods> transport_methods { get; set;}
    }
}