namespace ShipManagement.Models.Request
{
    public class CreateShipRequest
    {

        [ValidShipCode]
        public required string ShipCode { get; set; }
        [MaxLength(30)]
        public required string Name { get; set; }

        [Range(0.01, 50.0, ErrorMessage = "Velocity must be between 0.01 and 50.0 knots.")]
        public required decimal Velocity { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public required decimal Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public required decimal Longitude { get; set; }
    }
}