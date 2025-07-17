namespace ShipManagement.Models.DTOs
{
    public class ShipBasicDto
    {
        public int Id { get; set; }
        public string ShipCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Velocity { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class ShipDetailDto : ShipBasicDto
    {
        public List<UserDetailDto> AssignedUsers { get; set; } = new List<UserDetailDto>();
    }

    public class ShipDetailDtoWithBasicUsers : ShipBasicDto
    {
        public List<UserBasicDto> AssignedUsers { get; set; } = new List<UserBasicDto>();

    }
}