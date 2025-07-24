namespace ShipManagement.Models.Responses
{
    public class ShipDetails : ShipResponse
    {
        public List<UserBasicDto> AssignedUsers { get; set; } = new List<UserBasicDto>();
    }
}