
using System.ComponentModel.DataAnnotations;

namespace ShipManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserShip> UserShips { get; set; } = new List<UserShip>();
    }
}