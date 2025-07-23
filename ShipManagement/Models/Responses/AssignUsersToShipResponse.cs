namespace ShipManagement.Models.Responses
{
    public class AssignUsersToShipResponse
    {
        public int UserId { get; set; }
        public int ShipId { get; set; }
        public required string ShipCode { get; set; }
        public required string ShipName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}