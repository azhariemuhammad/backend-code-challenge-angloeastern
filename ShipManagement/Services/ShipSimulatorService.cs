namespace ShipManagement.Services
{
    public class ShipSimulatorService(
        IServiceProvider serviceProvider,
        IMessagePublisher messagePublisher,
        ILogger<ShipSimulatorService> logger) : BackgroundService
    {
        private const int UPDATE_INTERVAL_SECONDS = 30;
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Ship Simulator Service Started");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await SimulateShipMovements(cancellationToken);
                    await Task.Delay(30000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in ship simulation");
                }
            }
        }

        private async Task SimulateShipMovements(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShipManagementContext>();

            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var ships = await context.Ships.ToListAsync(cancellationToken);
                var messages = new List<(ShipPositionUpdate message, string routingKey)>();

                // First: Update ship positions in memory
                foreach (var ship in ships)
                {
                    var (newLat, newLon) = ShipPositionCalculator.CalculateNewPosition(
                        ship.Latitude,
                        ship.Longitude,
                        ship.Velocity,
                        ship.Heading,
                        UPDATE_INTERVAL_SECONDS
                    );

                    ship.Latitude = newLat;
                    ship.Longitude = newLon;
                    ship.UpdatedAt = DateTime.UtcNow;

                    // Prepare message for later publishing
                    var message = new ShipPositionUpdate
                    {
                        MessageId = Random.Shared.Next(1, int.MaxValue),
                        ShipCode = ship.ShipCode,
                        Latitude = newLat,
                        Longitude = newLon,
                        Velocity = ship.Velocity,
                        Heading = ship.Heading,
                        Timestamp = DateTime.UtcNow
                    };

                    messages.Add((message, $"ship.position.{ship.ShipCode}"));
                }

                // Second: Save all changes to database
                await context.SaveChangesAsync(cancellationToken);

                // Third: Publish messages to RabbitMQ (only after successful DB save)
                foreach (var (message, routingKey) in messages)
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(message);
                        await messagePublisher.PublishAsync(json, routingKey);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to publish position update for ship {ShipCode}", message.ShipCode);
                        // Continue with other messages - don't fail the whole transaction
                    }
                }

                // Fourth: Commit transaction
                await transaction.CommitAsync(cancellationToken);

                logger.LogInformation("Completed position updates for {ShipCount} ships", ships.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to simulate ship movements - rolling back transaction");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}