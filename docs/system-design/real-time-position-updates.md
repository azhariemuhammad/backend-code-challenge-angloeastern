# Real-time Ship Position Updates System Design

## Overview

This document outlines the system design for implementing real-time ship position updates with 30-second intervals using RabbitMQ message queuing in the Ship Management System.

## System Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│  Ship Simulator │───▶│   RabbitMQ       │───▶│ Position        │
│  (Background    │    │   Exchange       │    │ Consumer        │
│   Service)      │    │  + Queues        │    │ Service         │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       ▼
         │                       │              ┌─────────────────┐
         │                       │              │   Database      │
         │                       │              │  + Redis Cache  │
         │                       │              └─────────────────┘
         │                       │                       │
         │                       ▼                       │
         │              ┌──────────────────┐            │
         │              │ Other Consumers: │◀───────────┘
         │              │ • Distance Calc  │
         │              │ • Alert Service  │
         │              │ • Analytics      │
         │              │ • WebSocket      │
         │              └──────────────────┘
         │
         ▼
┌─────────────────┐
│   Admin API     │
│ (Start/Stop     │
│  Simulation)    │
└─────────────────┘
```

## Core Components

### 1. Ship Simulator Service (IHostedService)

**Purpose**: Background service that simulates realistic ship movement every 30 seconds.

**Key Responsibilities**:
- Generate new GPS coordinates based on current position, velocity, and heading
- Simulate realistic maritime routes (avoiding land, following shipping lanes)
- Publish position updates to RabbitMQ message queue
- Handle service lifecycle (start/stop with application)

**Technical Implementation**:
- Inherits from `BackgroundService` or implements `IHostedService`
- Uses `Timer` or `Task.Delay` for 30-second intervals
- Utilizes `IServiceScopeFactory` to access scoped services (database context)
- Implements proper error handling and logging

**Behavior Patterns**:
- Realistic movement calculations based on ship velocity
- Direction changes following maritime traffic patterns
- Speed variations within realistic ranges
- Handling of stationary periods (port stays)

### 2. RabbitMQ Message Infrastructure

**Exchange Strategy**:
```
ship.events (Topic Exchange)
├── ship.position.updates (routing key)
├── ship.velocity.changes (routing key)
├── ship.alerts.proximity (routing key)
├── ship.alerts.deviation (routing key)
└── ship.arrivals.port (routing key)
```

**Queue Configuration**:
- **position-updates-queue**: High-throughput queue for position data
- **distance-calculation-queue**: Background distance calculations
- **alert-processing-queue**: Proximity alerts and notifications
- **analytics-queue**: Data aggregation and reporting
- **websocket-updates-queue**: Real-time frontend updates

**Message Durability**:
- Persistent messages to survive broker restarts
- Dead letter queues for failed processing
- TTL (Time To Live) of 5 minutes for position messages
- Manual acknowledgment for reliable processing

### 3. Position Consumer Service (IHostedService)

**Primary Functions**:
- Consume position update messages from RabbitMQ
- Update ship positions in PostgreSQL database
- Cache latest positions in Redis for fast API responses
- Trigger downstream processing (distance calculations, alerts)

**Processing Logic**:
```csharp
// Pseudo-code for position processing
public async Task ProcessPositionUpdate(ShipPositionUpdate message)
{
    // 1. Validate message data
    if (!IsValidPosition(message)) return;
    
    // 2. Update database
    await UpdateShipPosition(message);
    
    // 3. Update cache
    await CacheLatestPosition(message);
    
    // 4. Trigger distance recalculation
    await PublishDistanceCalculationRequest(message);
    
    // 5. Check for alerts
    await CheckProximityAlerts(message);
}
```

**Error Handling**:
- Retry logic with exponential backoff
- Dead letter queue for persistently failing messages
- Circuit breaker pattern for database connections
- Comprehensive logging and monitoring

### 4. Distance Calculation Consumer

**Trigger Events**: Position updates, new port additions, configuration changes

**Calculation Logic**:
- Recalculate closest port for ships with significant position changes
- Update estimated arrival times based on current velocity
- Detect ships approaching ports (configurable threshold, default 10km)
- Optimize calculations by skipping minor position changes

**Performance Optimizations**:
- Distance threshold for triggering recalculations (e.g., 1km movement)
- Caching of port coordinates for faster calculations
- Batch processing of multiple position updates
- Asynchronous processing to avoid blocking

### 5. Alert Service Consumer

**Alert Types**:
- **Proximity Alerts**: Ship approaching port (< 10km threshold)
- **Deviation Alerts**: Ship moving away from expected route
- **Stationary Alerts**: Ship stopped for extended period (> 2 hours)
- **Emergency Alerts**: Distress signals or unusual patterns

**Notification Channels**:
- Real-time WebSocket updates to connected clients
- Email notifications for critical alerts
- SMS for emergency situations
- Dashboard notifications
- External system integrations (port authorities, customs)

## Data Models

### Position Update Message
```json
{
  "messageId": "uuid-v4",
  "shipCode": "SHIP001",
  "position": {
    "latitude": 1.2966,
    "longitude": 103.7764,
    "accuracy": 3.5
  },
  "movement": {
    "velocity": 15.5,
    "heading": 045,
    "speedOverGround": 14.8
  },
  "timestamp": "2025-08-05T10:30:00Z",
  "source": "GPS_SIMULATOR",
  "metadata": {
    "simulationRun": "SIM_2025080501",
    "sequenceNumber": 1205
  }
}
```

### Alert Message
```json
{
  "alertId": "uuid-v4",
  "alertType": "PROXIMITY_ALERT",
  "shipCode": "SHIP001",
  "severity": "MEDIUM",
  "message": "Ship SHIP001 approaching Port of Singapore",
  "data": {
    "portName": "Port of Singapore",
    "distanceKm": 8.5,
    "estimatedArrival": "2025-08-05T12:15:00Z"
  },
  "timestamp": "2025-08-05T10:30:00Z"
}
```

## Performance Specifications

### Throughput Requirements
- **Ships**: 100-1000 ships in system
- **Update Frequency**: Every 30 seconds per ship
- **Message Volume**: 120-1200 messages per minute
- **Peak Load**: 20-200 messages per second during bulk updates
- **Response Time**: Position updates processed within 5 seconds
- **API Response**: Position queries from cache < 100ms

### Scalability Targets
- **Horizontal Scaling**: Support multiple consumer instances
- **Database Load**: Handle 1000+ concurrent position updates
- **Cache Performance**: Redis hit rate > 95% for position queries
- **Queue Depth**: Maintain processing lag < 30 seconds under normal load

## Implementation Strategy

### Phase 1: Core Infrastructure
1. Set up RabbitMQ with basic exchange and queues
2. Implement ship simulator service with basic position generation
3. Create position consumer service with database updates
4. Basic error handling and logging

### Phase 2: Enhanced Features
1. Add Redis caching layer for fast position retrieval
2. Implement distance calculation consumer
3. Add proximity alert system
4. WebSocket integration for real-time updates

### Phase 3: Production Readiness
1. Comprehensive monitoring and health checks
2. Performance optimization and tuning
3. Advanced error handling and recovery
4. Load testing and scalability validation

### Single Application Architecture

The system is designed as a **single .NET application** with multiple background services:

```csharp
// Program.cs service registration
builder.Services.AddHostedService<ShipSimulatorService>();
builder.Services.AddHostedService<PositionConsumerService>();
builder.Services.AddHostedService<DistanceCalculationService>();
builder.Services.AddHostedService<AlertProcessingService>();

// Shared services
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddStackExchangeRedisCache(options => 
    options.Configuration = connectionString);
```

**Benefits**:
- Shared database context and models
- Simplified deployment and debugging
- Efficient resource utilization
- Consistent logging and configuration

## Configuration Management

### Simulation Parameters
```json
{
  "ShipSimulator": {
    "UpdateIntervalSeconds": 30,
    "MovementVariation": 0.1,
    "SpeedVariationPercent": 10,
    "DirectionChangeChance": 0.05,
    "GeographicBounds": {
      "MinLatitude": -90,
      "MaxLatitude": 90,
      "MinLongitude": -180,
      "MaxLongitude": 180
    }
  }
}
```

### RabbitMQ Settings
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ExchangeName": "ship.events",
    "QueueSettings": {
      "MessageTTL": 300000,
      "MaxRetries": 3,
      "RetryDelay": 5000
    }
  }
}
```

### Alert Thresholds
```json
{
  "AlertSettings": {
    "ProximityThresholdKm": 10.0,
    "StationaryThresholdMinutes": 120,
    "DeviationThresholdKm": 5.0,
    "EmergencySpeedThreshold": 0.5
  }
}
```

## Monitoring and Observability

### Key Metrics
- **Message Throughput**: Messages per second processed
- **Queue Depth**: Backlog in each message queue
- **Processing Latency**: Time from message publish to processing
- **Database Performance**: Query execution times and connection pool usage
- **Cache Hit Rate**: Redis cache effectiveness
- **Error Rates**: Failed message processing percentage

### Health Checks
- RabbitMQ connection status
- Database connectivity
- Redis cache availability
- Background service health
- Message processing lag

### Logging Strategy
- Structured logging with correlation IDs
- Different log levels for development vs production
- Performance logging for slow operations
- Error tracking with stack traces
- Audit logging for position updates

## Fault Tolerance and Recovery

### Failure Scenarios and Mitigations

**RabbitMQ Broker Failure**:
- Publisher retry with exponential backoff
- Circuit breaker pattern for message publishing
- Local queue for temporary message storage
- Automatic reconnection on broker recovery

**Database Connectivity Issues**:
- Connection pool retry logic
- Graceful degradation (continue processing, queue failed updates)
- Health check monitoring
- Automatic failover to read replicas

**Consumer Service Crashes**:
- Messages return to queue for reprocessing
- Supervisor restart policies
- Health monitoring and alerting
- Graceful shutdown handling

**Network Partitions**:
- Message deduplication using unique IDs
- Idempotent message processing
- State reconciliation on reconnection
- Conflict resolution strategies

## Security Considerations

### Message Security
- TLS encryption for RabbitMQ connections
- Message signing for integrity verification
- Access control for queue operations
- Rate limiting to prevent abuse

### Data Protection
- Encryption at rest for sensitive position data
- Audit logging for all position updates
- Data retention policies
- GDPR compliance for personal data

## Future Enhancements

### Advanced Features
- Machine learning for route prediction
- Weather integration for route optimization
- Port capacity management
- Fuel consumption optimization
- Predictive maintenance alerts

### Scalability Improvements
- Horizontal sharding of ship data
- Geographic partitioning of message queues
- Event sourcing for complete position history
- CQRS pattern for read/write optimization

### Integration Opportunities
- AIS (Automatic Identification System) data feeds
- Weather service APIs
- Port management systems
- Customs and immigration systems
- Supply chain management platforms

## Conclusion

This system design provides a robust, scalable foundation for real-time ship position tracking using modern messaging patterns. The use of RabbitMQ enables loose coupling between components while maintaining high throughput and reliability. The single-application architecture simplifies deployment while the modular service design enables future scaling and enhancement opportunities.
