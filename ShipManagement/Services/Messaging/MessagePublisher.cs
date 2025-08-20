using Microsoft.Extensions.Options;
using ShipManagement.Configuration;
using RabbitMQ.Client;
using ShipManagement.Interfaces;
using System.Text;
using System.Text.Json;

// // TODO: Handle exceptions like JsonException, InvalidOperationException, TimeoutException
// Serialization problems with ShipPositionUpdate messages
// RabbitMQ connection issues during simulation
// Timeout issues under high message load
// Unexpected errors during message publishing

namespace ShipManagement.Services.Messaging
{
    public class MessagePublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly RabbitMQOptions _options;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _initialized = false;

        public MessagePublisher(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;
        }

        private async Task InitializeAsync()
        {
            if (_initialized) return;

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync("ship.events", ExchangeType.Topic, durable: true);

            _initialized = true;
        }

        public async Task<string> PublishAsync(string message, string routingKey = "")
        {
            await InitializeAsync();

            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                await _channel.BasicPublishAsync("ship.events", routingKey, body);
                return $"Published: {message}";
            }
            catch (Exception ex)
            {
                // Handle your TODO exceptions here
                throw new InvalidOperationException("Publishing failed", ex);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync();
                _connection = null;
            }
        }
    }
}