using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class TransportMethods
    {
        public TransportType type { get; set; }
        public string name { get; set; }
        public string estimated_days { get; set; }
    }
} 