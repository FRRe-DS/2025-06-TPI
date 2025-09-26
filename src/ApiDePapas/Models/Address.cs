using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class Address
    {
        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [RegularExpression(@"^([A-Z]{1}\d{4}[A-Z]{3})$", ErrorMessage = "Invalid postal code format")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be 2 characters")]
        public string Country { get; set; }
    }
}
