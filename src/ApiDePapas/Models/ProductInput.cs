using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ApiDePapas.Models
{
    public class ProductInput
    {
        [Required]
        public int product_id { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}