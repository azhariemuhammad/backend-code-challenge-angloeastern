namespace ShipManagement.Models.Responses
{
    public class GetUserResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<ShipResponse> AssignedShips { get; set; } = new List<ShipResponse>();

    }
}