using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class Address
    {
        [Required]
        public string street { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string state { get; set; }

        [Required]
        [RegularExpression(@"^([A-Z]{1}\d{4}[A-Z]{3})$", ErrorMessage = "Invalid postal code format")]
        public string postal_code { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be 2 characters")]
        public string country { get; set; }
    }
}
