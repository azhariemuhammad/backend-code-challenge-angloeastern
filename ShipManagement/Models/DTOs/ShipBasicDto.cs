namespace ShipManagement.Models.DTOs
{
    public class ShipBasicDto
    {
        public int Id { get; set; }
        public string ShipId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Velocity { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class ShipDetailDto : ShipBasicDto
    {
        public List<UserDto> AssignedUsers { get; set; } = new List<UserDto>();
    }
}