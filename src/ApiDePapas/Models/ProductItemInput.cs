using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ApiDePapas.Models
{
    public class ProductItemInput
    {
        [Required]
        public BigInteger Id { get; set; }

        [Required]
        public BigInteger Quantity { get; set; }

        [Required]
        public float Weight { get; set; }

        [Required]
        public float Length { get; set; }

        [Required]
        public float Width { get; set; }

        [Required]
        public float Height { get; set; }
    }
}