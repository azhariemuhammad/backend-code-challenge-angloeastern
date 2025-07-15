namespace ShipManagement.Models
{
    public class UserShip
    {
        public int UserId { get; set; }
        public int ShipId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        public Ship Ship { get; set; } = null!;
    }
}