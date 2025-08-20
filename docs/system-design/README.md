# System Design Documentation

## Real-time Ship Position Updates (30-second intervals)

### Overview
This system design implements real-time ship position tracking using RabbitMQ message queues with position updates every 30 seconds. The architecture uses a single .NET application with background services to simulate and process ship movements.

### Architecture

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

### Components

#### 1. Ship Simulator Service (IHostedService)
- **Purpose**: Background service that generates realistic ship position updates
- **Frequency**: Every 30 seconds
- **Behavior**: 
  - Simulates movement based on ship velocity and heading
  - Generates GPS coordinates (latitude, longitude)
  - Publishes position updates to RabbitMQ
- **Implementation**: Uses `BackgroundService` with Timer

#### 2. RabbitMQ Infrastructure
- **Exchange**: `ship.events` (Topic Exchange)
- **Routing Keys**: 
  - `ship.position.updates`
  - `ship.velocity.changes`
  - `ship.alerts.*`
  - `ship.arrivals.*`
- **Queues**:
  - `position-updates-queue`: High-throughput position data
  - `distance-calculation-queue`: Background distance calculations
  - `alert-processing-queue`: Proximity alerts and notifications
  - `analytics-queue`: Data analytics and reporting

#### 3. Position Consumer Service (IHostedService)
- **Purpose**: Processes incoming position update messages
- **Responsibilities**:
  - Update ship positions in PostgreSQL database
  - Cache latest positions in Redis for fast retrieval
  - Trigger distance recalculations
  - Detect significant position changes

#### 4. Distance Calculation Consumer
- **Trigger**: Position update events
- **Logic**: 
  - Recalculate closest port for each ship
  - Update estimated arrival times
  - Detect ships approaching ports (within configurable threshold)
- **Optimization**: Only recalculate if ship moved significantly

#### 5. Alert Service Consumer
- **Monitors**: Position changes for alert conditions
- **Alert Types**:
  - Ship approaching port (< 10km)
  - Ship deviating from expected route
  - Ship stopped for extended period
  - Emergency/distress situations

### Data Flow

1. **Timer Triggers** (every 30 seconds)
2. **Ship Simulator** generates new positions for all active ships
3. **Publisher** sends `ShipPositionUpdate` message to RabbitMQ
4. **Position Consumer** receives message and updates database + cache
5. **Secondary Consumers** process for distance calculations, alerts, etc.

### Message Structure

```json
{
  "shipCode": "SHIP001",
  "latitude": 1.2966,
  "longitude": 103.7764,
  "velocity": 15.5,
  "heading": 045,
  "timestamp": "2025-08-05T10:30:00Z",
  "accuracy": 3.5,
  "source": "GPS_SIMULATOR"
}
```

### Performance Characteristics

- **Ships**: 100-1000 ships in system
- **Update Frequency**: Every 30 seconds
- **Message Volume**: 120-1200 messages per minute
- **Peak Load**: 20-200 messages per second during bulk updates

### Technology Stack

- **Application**: Single .NET 9 Web API with Background Services
- **Message Broker**: RabbitMQ
- **Database**: PostgreSQL (persistent storage)
- **Cache**: Redis (fast position retrieval)
- **Background Processing**: IHostedService/BackgroundService
- **Dependency Injection**: Built-in .NET DI Container

### Deployment

- **Single Container**: All services run in one .NET application
- **Docker Compose**: Includes RabbitMQ, PostgreSQL, Redis, and the main application
- **Scaling**: Horizontal scaling through multiple container instances
- **Configuration**: Environment variables for different deployment scenarios

### Benefits

1. **Simplified Architecture**: Single application reduces operational complexity
2. **Shared Resources**: Common database context and models across all services
3. **Real-time Processing**: Asynchronous message handling for high throughput
4. **Fault Tolerance**: Message persistence and retry mechanisms
5. **Scalability**: Queue-based architecture supports horizontal scaling
6. **Monitoring**: Centralized logging and metrics collection

### Future Enhancements

- WebSocket integration for real-time frontend updates
- Geographic boundary validation
- Route optimization algorithms
- Integration with external weather services
- Advanced analytics and reporting dashboards

