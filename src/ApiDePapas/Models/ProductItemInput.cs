using System.ComponentModel.DataAnnotations;
using System.Numerics;

//deberiamos borrar esto, no tenemos esos datos (los pedimos a stock)

namespace ApiDePapas.Models
{
    public class ProductItemInput
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

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