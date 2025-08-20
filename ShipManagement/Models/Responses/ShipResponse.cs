namespace ShipManagement.Models.Responses
{
    public class ShipResponse
    {
        public required int Id { get; set; }
        public required string ShipCode { get; set; }
        public required string Name { get; set; }
        public required decimal Velocity { get; set; }
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
        public int Heading { get; set; } = 0;
    }
}