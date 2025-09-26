using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models

{
    public class CreateShippingRequest
    {
        [Required]
        public int user_id { get; set; }

        [Required]
        public Address delivery_address { get; set; }

        [Required]
        public string departure_postal_code { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one product is required")]
        public List<ProductItemInput> product { get; set; }
    }
}