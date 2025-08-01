namespace ShipManagement.Models.DTOs
{
    public class UserBasicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }

    public class UserDetailDto : UserBasicDto
    {
        public List<ShipBasicDto> AssignedShips { get; set; } = new List<ShipBasicDto>();
    }
}