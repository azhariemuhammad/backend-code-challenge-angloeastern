namespace ShipManagement.Models.DTOs
{
    public class PortWithDistanceDto
    {
        public Port Port { get; set; }
        public double DistanceKm { get; set; }
    }

    public class EstimatedArrivalDto
    {
        public Ship Ship { get; set; }
        public Port ClosestPort { get; set; }
        public double DistanceKm { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public double TravelTimeHours { get; set; }
    }
}