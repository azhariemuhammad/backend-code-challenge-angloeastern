using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models
{
    public class Ship
    {
        public int Id { get; set; }
        [Required]
        public string ShipId { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public decimal Velocity { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserShip> UserShips { get; set; } = new List<UserShip>();
    }
}