namespace ShipManagement.Interfaces
{
    public interface IMessagePublisher
    {
        Task<string> PublishAsync(string message, string routingKey = "");
    }
}