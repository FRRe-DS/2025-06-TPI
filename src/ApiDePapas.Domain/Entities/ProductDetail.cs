using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.Entities;

public class ProductDetail
{
    [Required]
    public int id { get; set; }

    [Required]
    public float weight { get; set; }

    [Required]
    public float length { get; set; }

    [Required]
    public float width { get; set; }

    [Required]
    public float height { get; set; }
}