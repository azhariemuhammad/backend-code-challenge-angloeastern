namespace ShipManagement.Interfaces
{
    public interface IMessage
    {
        int MessageId { get; set; }
        DateTime Timestamp { get; set; }
    }
}