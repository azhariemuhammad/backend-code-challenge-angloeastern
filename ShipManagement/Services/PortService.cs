using ShipManagement.Models.DTOs;
using ShipManagement.Models;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Interfaces;
using ShipManagement.Data;

namespace ShipManagement.Services

{
    public class PortService : IPortService
    {
        private readonly ShipManagementContext _context;
        public PortService(ShipManagementContext context)
        {
            _context = context;
        }

        public async Task<PortWithDistanceDto> GetClosestPortAsync(int shipId)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null) throw new KeyNotFoundException("Ship not found");

            var shipLatitude = ship.Latitude;
            var shipLongitude = ship.Longitude;
            var ports = await _context.Ports.ToListAsync();

            var closestPort = ports
                .Select(port => new PortWithDistanceDto
                {
                    Port = port,
                    DistanceKm = DistanceCalculator.CalculateDistanceKm(
                        shipLatitude, shipLongitude,
                        port.Latitude, port.Longitude)
                })
                .OrderBy(p => p.DistanceKm)
                .FirstOrDefault();

            return closestPort;
        }

        public async Task<EstimatedArrivalDto> GetEstimatedArrivalAsync(int shipId)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null) throw new KeyNotFoundException("Ship not found");
            // Get the closest port
            var closestPort = await GetClosestPortAsync(shipId);
            if (closestPort == null) throw new KeyNotFoundException("No ports found");
            {

                // Calculate estimated arrival time
                var velocityKmh = (double)ship.Velocity * 1.852; // 1 knot = 1.852 km/h
                var estimatedHours = closestPort.DistanceKm / velocityKmh;
                var estimatedArrival = DateTime.UtcNow.AddHours(estimatedHours);

                return new EstimatedArrivalDto
                {
                    Ship = ship,
                    ClosestPort = closestPort.Port,
                    DistanceKm = closestPort.DistanceKm,
                    EstimatedArrivalTime = estimatedArrival,
                    TravelTimeHours = estimatedHours
                };
            }
        }
    }
}