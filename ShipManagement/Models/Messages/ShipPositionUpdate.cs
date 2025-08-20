namespace ShipManagement.Models.Messages
{
    public class ShipPositionUpdate : IMessage
    {
        public required int MessageId { get; set; }
        public required string ShipCode { get; set; }
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
        public decimal Velocity { get; set; }

        [Required, Range(0, 360)]
        public int Heading { get; set; }
        public required DateTime Timestamp { get; set; }
    }
}