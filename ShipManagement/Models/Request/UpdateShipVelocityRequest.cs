namespace ShipManagement.Models.Request
{
    public class UpdateShipVelocityRequest
    {
        [Range(0.01, 50.0, ErrorMessage = "Velocity must be between 0.01 and 50.0 knots.")]
        public required decimal Velocity { get; set; }
    }
}