# RabbitMQ Message Queue Architecture

## Overview

This document details the RabbitMQ messaging architecture for the Ship Management System, focusing on exchange patterns, queue design, and message flow.

## Exchange Strategy

### Topic Exchange: `ship.events`

We use a **Topic Exchange** pattern to enable flexible message routing based on routing keys.

```
ship.events (Topic Exchange)
├── ship.position.updates          # Real-time position data
├── ship.velocity.changes          # Speed/direction changes
├── ship.alerts.proximity          # Ships approaching ports
├── ship.alerts.deviation          # Route deviation alerts
├── ship.alerts.stationary         # Ships stopped for extended periods
├── ship.arrivals.scheduled        # Scheduled port arrivals
├── ship.arrivals.actual           # Actual arrival confirmations
└── ship.maintenance.*             # Maintenance-related events
```

### Routing Key Patterns

**Hierarchical Structure**: `entity.action.subtype`

Examples:
- `ship.position.updates` - Position update messages
- `ship.alerts.proximity` - Proximity alert messages
- `ship.arrivals.scheduled` - Scheduled arrival notifications
- `ship.maintenance.due` - Maintenance due alerts

**Benefits**:
- Consumers can subscribe to specific message types
- Wildcard subscriptions (e.g., `ship.alerts.*` for all alerts)
- Easy to add new message types without changing existing consumers

## Queue Design

### Primary Queues

#### 1. Position Updates Queue
```json
{
  "queueName": "position-updates-queue",
  "routingKey": "ship.position.updates",
  "properties": {
    "durable": true,
    "autoDelete": false,
    "messageTTL": 300000,
    "maxLength": 10000,
    "deadLetterExchange": "ship.events.dlx"
  }
}
```

**Characteristics**:
- High throughput (100+ messages/minute)
- Short TTL (5 minutes) - position data becomes stale quickly
- Bounded queue to prevent memory issues
- Dead letter queue for failed processing

#### 2. Distance Calculation Queue
```json
{
  "queueName": "distance-calculation-queue",
  "routingKey": "ship.position.updates",
  "properties": {
    "durable": true,
    "autoDelete": false,
    "messageTTL": 600000,
    "prefetchCount": 50
  }
}
```

**Characteristics**:
- Processes same position updates for calculations
- Longer TTL (10 minutes) for processing flexibility
- Higher prefetch count for batch processing efficiency

#### 3. Alert Processing Queue
```json
{
  "queueName": "alert-processing-queue",
  "routingKey": "ship.alerts.*",
  "properties": {
    "durable": true,
    "autoDelete": false,
    "messageTTL": 3600000,
    "priority": true,
    "maxPriority": 10
  }
}
```

**Characteristics**:
- Processes all alert types with wildcard routing
- Longer TTL (1 hour) for alert persistence
- Priority queue for critical alerts
- High durability requirements

#### 4. Analytics Queue
```json
{
  "queueName": "analytics-queue",
  "routingKey": "ship.#",
  "properties": {
    "durable": true,
    "autoDelete": false,
    "messageTTL": 86400000,
    "lazy": true
  }
}
```

**Characteristics**:
- Subscribes to all ship events
- Long TTL (24 hours) for batch processing
- Lazy queue for memory efficiency
- Used for reporting and analytics

#### 5. WebSocket Updates Queue
```json
{
  "queueName": "websocket-updates-queue",
  "routingKey": "ship.position.updates",
  "properties": {
    "durable": false,
    "autoDelete": true,
    "messageTTL": 60000,
    "exclusive": true
  }
}
```

**Characteristics**:
- Real-time updates for connected clients
- Short TTL (1 minute) - real-time data only
- Non-durable for performance
- Auto-delete when no consumers

### Dead Letter Queues

#### Dead Letter Exchange
```json
{
  "exchangeName": "ship.events.dlx",
  "exchangeType": "direct",
  "queues": [
    {
      "queueName": "failed-position-updates",
      "routingKey": "position-updates-queue"
    },
    {
      "queueName": "failed-alerts",
      "routingKey": "alert-processing-queue"
    }
  ]
}
```

**Purpose**:
- Capture messages that fail processing after max retries
- Enable manual inspection and reprocessing
- Maintain system stability by preventing infinite retry loops

## Message Flow Patterns

### 1. Position Update Flow

```
Ship Simulator → [Publish] → ship.events Exchange
                                    ↓ (ship.position.updates)
                            ┌─────────────────┐
                            │ Multiple Queues │
                            │ • Position      │
                            │ • Distance      │
                            │ • WebSocket     │
                            │ • Analytics     │
                            └─────────────────┘
                                    ↓
                            ┌─────────────────┐
                            │ Parallel        │
                            │ Consumers       │
                            └─────────────────┘
```

### 2. Alert Generation Flow

```
Position Consumer → [Detect Alert] → [Publish] → ship.events Exchange
                                                        ↓ (ship.alerts.proximity)
                                                Alert Processing Queue
                                                        ↓
                                                Alert Consumer
                                                        ↓
                                        ┌─────────────────────────┐
                                        │ Notification Channels   │
                                        │ • WebSocket             │
                                        │ • Email                 │
                                        │ • SMS                   │
                                        │ • Dashboard             │
                                        └─────────────────────────┘
```

### 3. Fan-out Pattern

**Single Message → Multiple Consumers**:
- One position update triggers multiple processing workflows
- Each consumer processes independently
- Failure in one consumer doesn't affect others
- Enables adding new consumers without modifying publishers

## Message Schemas

### Position Update Message
```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "messageId": {
      "type": "string",
      "format": "uuid"
    },
    "shipCode": {
      "type": "string",
      "pattern": "^[A-Z0-9]{3,10}$"
    },
    "position": {
      "type": "object",
      "properties": {
        "latitude": {
          "type": "number",
          "minimum": -90,
          "maximum": 90
        },
        "longitude": {
          "type": "number",
          "minimum": -180,
          "maximum": 180
        },
        "accuracy": {
          "type": "number",
          "minimum": 0
        }
      },
      "required": ["latitude", "longitude"]
    },
    "movement": {
      "type": "object",
      "properties": {
        "velocity": {
          "type": "number",
          "minimum": 0
        },
        "heading": {
          "type": "number",
          "minimum": 0,
          "maximum": 359
        }
      }
    },
    "timestamp": {
      "type": "string",
      "format": "date-time"
    },
    "source": {
      "type": "string",
      "enum": ["GPS_SIMULATOR", "AIS", "MANUAL", "SATELLITE"]
    }
  },
  "required": ["messageId", "shipCode", "position", "timestamp", "source"]
}
```

### Alert Message
```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "alertId": {
      "type": "string",
      "format": "uuid"
    },
    "alertType": {
      "type": "string",
      "enum": ["PROXIMITY", "DEVIATION", "STATIONARY", "EMERGENCY"]
    },
    "severity": {
      "type": "string",
      "enum": ["LOW", "MEDIUM", "HIGH", "CRITICAL"]
    },
    "shipCode": {
      "type": "string"
    },
    "message": {
      "type": "string",
      "maxLength": 500
    },
    "data": {
      "type": "object"
    },
    "timestamp": {
      "type": "string",
      "format": "date-time"
    },
    "expiresAt": {
      "type": "string",
      "format": "date-time"
    }
  },
  "required": ["alertId", "alertType", "severity", "shipCode", "message", "timestamp"]
}
```

## Performance Tuning

### Connection Management
```csharp
// Connection pooling settings
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "shipmanagement",
    Password = "secretpassword",
    VirtualHost = "/",
    
    // Performance settings
    RequestedHeartbeat = TimeSpan.FromSeconds(60),
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
    AutomaticRecoveryEnabled = true,
    
    // Threading settings
    DispatchConsumersAsync = true,
    ConsumerDispatchConcurrency = 5
};
```

### Channel Management
```csharp
// Channel per consumer for better isolation
public class PositionConsumerService : BackgroundService
{
    private IModel _channel;
    private IConnection _connection;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Configure prefetch for better throughput
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 50, global: false);
        
        // Set up consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += ProcessMessage;
        
        _channel.BasicConsume(
            queue: "position-updates-queue",
            autoAck: false,  // Manual acknowledgment for reliability
            consumer: consumer
        );
    }
}
```

### Message Batching
```csharp
public async Task PublishBatch(IEnumerable<PositionUpdate> updates)
{
    var batch = _channel.CreateBasicPublishBatch();
    
    foreach (var update in updates)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(update);
        batch.Add(
            exchange: "ship.events",
            routingKey: "ship.position.updates",
            mandatory: false,
            properties: _channel.CreateBasicProperties(),
            body: body
        );
    }
    
    batch.Publish();
}
```

## Monitoring and Metrics

### Queue Metrics
- **Message Rate**: Messages per second in/out
- **Queue Depth**: Number of unprocessed messages
- **Consumer Count**: Active consumers per queue
- **Message TTL**: Time to live for messages
- **Dead Letter Count**: Failed message count

### Connection Metrics
- **Connection Count**: Active connections to broker
- **Channel Count**: Active channels per connection
- **Network I/O**: Bytes sent/received
- **Heartbeat Failures**: Connection health issues

### Performance Metrics
- **Processing Latency**: Time from publish to processing
- **Throughput**: Messages processed per second
- **Error Rate**: Failed message processing percentage
- **Memory Usage**: Broker memory consumption

### Health Checks
```csharp
public class RabbitMqHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            
            // Check if we can declare a temporary queue
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueDelete(queueName);
            
            return HealthCheckResult.Healthy("RabbitMQ is responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is not responsive", ex);
        }
    }
}
```

## Error Handling Strategies

### Retry Policies
```csharp
public class RetryPolicy
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromMinutes(5);
    public double BackoffMultiplier { get; set; } = 2.0;
}
```

### Circuit Breaker
```csharp
public class MessageProcessorWithCircuitBreaker
{
    private readonly CircuitBreaker _circuitBreaker;
    
    public async Task ProcessMessage(string message)
    {
        await _circuitBreaker.ExecuteAsync(async () =>
        {
            // Process the message
            await DoProcessMessage(message);
        });
    }
}
```

### Poison Message Handling
```csharp
public async Task HandleMessage(BasicDeliverEventArgs eventArgs)
{
    try
    {
        var message = JsonSerializer.Deserialize<PositionUpdate>(eventArgs.Body.Span);
        await ProcessPositionUpdate(message);
        
        // Acknowledge successful processing
        _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }
    catch (JsonException ex)
    {
        // Poison message - reject without requeue
        _logger.LogError(ex, "Invalid message format");
        _channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
    }
    catch (Exception ex)
    {
        // Temporary error - requeue for retry
        _logger.LogWarning(ex, "Processing failed, will retry");
        _channel.BasicReject(eventArgs.DeliveryTag, requeue: true);
    }
}
```

## Security Configuration

### Authentication and Authorization
```csharp
// Connection with credentials
var factory = new ConnectionFactory()
{
    HostName = "rabbitmq.shipmanagement.com",
    Port = 5671,  // TLS port
    UserName = "ship-service",
    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
    VirtualHost = "/ship-management",
    
    // TLS settings
    Ssl = new SslOption()
    {
        Enabled = true,
        ServerName = "rabbitmq.shipmanagement.com",
        CertificateValidationCallback = ValidateServerCertificate
    }
};
```

### Access Control
- **Publishers**: Ship simulator service, alert generators
- **Consumers**: Position processors, distance calculators, alert handlers
- **Administrators**: Queue management, monitoring tools
- **Read-only**: Monitoring and analytics services

This messaging architecture provides a robust, scalable foundation for handling real-time ship position updates and related events in the Ship Management System.
