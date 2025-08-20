namespace ShipManagement.Configuration
{
    public class RabbitMQOptions
    {
        public string ConnectionString { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int ConnectionTimeout { get; set; }
        public int RequestedHeartbeat { get; set; }
        public int NetworkRecoveryInterval { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public int Retry { get; set; }
        public int RetryDelay { get; set; }
    }
}