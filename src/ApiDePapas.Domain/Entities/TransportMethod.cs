using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.Entities{
    public class TransportMethod{

        [Required]
        public int transport_id { get; set; }

        [Required]
        public float average_speed { get; set; }

        [Required]
        public bool available { get; set; }

        [Required]
        public float max_capacity { get; set; }

    }
}