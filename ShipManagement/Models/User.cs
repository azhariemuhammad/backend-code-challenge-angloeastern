
using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Ship> Ships { get; set; } = new List<Ship>();
    }
}