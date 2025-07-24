namespace ShipManagement.Exceptions
{
    public class DuplicateShipCodeException : Exception
    {
        public DuplicateShipCodeException(string message) : base(message) { }
    }
}
