namespace ShipManagement.Exceptions
{
    public class DuplicateUserNameException : Exception
    {
        public DuplicateUserNameException(string message) : base(message) { }
    }
}
