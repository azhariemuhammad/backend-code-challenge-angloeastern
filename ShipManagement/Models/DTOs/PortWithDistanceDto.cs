namespace ShipManagement.Models.DTOs
{
    public class PortWithDistanceDto
    {
        public required Port Port { get; set; }
        public double DistanceKm { get; set; }
    }
}