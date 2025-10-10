using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class TransportMethods
    {
        public TransportType type { get; set; }

        public string name { get; set; } = string.Empty;

        public string estimated_days { get; set; } = string.Empty;
    }
} 
