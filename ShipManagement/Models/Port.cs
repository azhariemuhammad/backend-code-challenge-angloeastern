using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models
{
    public class Port
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}