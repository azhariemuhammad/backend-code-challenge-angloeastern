namespace ShipManagement.Models.Responses
{
    public class ShipClosestPortResponse
    {
        public required string ShipCode { get; set; }
        public required string ShipName { get; set; }
        public required string PortName { get; set; }
        public required string PortCountry { get; set; }
        public decimal PortLatitude { get; set; }
        public decimal PortLongitude { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public double DistanceToPort { get; set; }
    }

}