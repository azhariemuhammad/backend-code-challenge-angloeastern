
using Microsoft.AspNetCore.Http.HttpResults;
using ShipManagement.Helpers;

namespace ShipManagement.Services
{
    public class ShipService(ShipManagementContext context) : IShipService
    {

        public async Task<IEnumerable<ShipResponse>> GetShipsAsync()
        {
            return await context.Ships
                .Select(s => new ShipResponse
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                })
                .ToListAsync();
        }

        public async Task<ShipResponse> CreateShipAsync(CreateShipRequest ship)
        {
            var existingShip = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == ship.ShipCode);
            if (existingShip != null)
            {
                throw new InvalidOperationException("Ship code is already in use.");
            }
            var newShip = new Ship
            {
                ShipCode = ship.ShipCode,
                Name = ship.Name,
                Velocity = ship.Velocity,
                Latitude = ship.Latitude,
                Longitude = ship.Longitude
            };
            context.Ships.Add(newShip);
            await context.SaveChangesAsync();
            return new ShipResponse
            {
                Id = newShip.Id,
                ShipCode = newShip.ShipCode,
                Name = newShip.Name,
                Velocity = newShip.Velocity,
                Latitude = newShip.Latitude,
                Longitude = newShip.Longitude
            };
        }

        public async Task<IEnumerable<AssignUsersToShipResponse>> AssignUsersToShipAsync(List<int> userIds, string shipCode)
        {
            var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            if (users.Count == 0)
            {
                throw new KeyNotFoundException("No users found with the provided IDs.");
            }
            var ship = await context.Ships.Include(s => s.Users).FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }
            foreach (var user in users)
            {
                if (!ship.Users.Any(u => u.Id == user.Id))
                {
                    ship.Users.Add(user);
                }
            }
            await context.SaveChangesAsync();
            return users.Select(user => new AssignUsersToShipResponse
            {
                UserId = user.Id,
                ShipCode = ship.ShipCode,
                ShipName = ship.Name
            }).ToList();
        }

        public async Task UnassignUsersFromShipAsync(List<int> userIds, string shipCode)
        {
            var ship = await context.Ships.Include(s => s.Users).FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }
            var usersToRemove = ship.Users.Where(u => userIds.Contains(u.Id)).ToList();
            if (usersToRemove.Count == 0)
            {
                throw new KeyNotFoundException("No user-ship assignments found for the provided user IDs and ship code.");
            }
            foreach (var user in usersToRemove)
            {
                ship.Users.Remove(user);
            }
            await context.SaveChangesAsync();
        }


        public async Task<ShipDetails?> GetShipByCodeAsync(string shipCode)
        {
            return await context.Ships
                .Where(s => s.ShipCode == shipCode)
                .Select(s => new ShipDetails
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    AssignedUsers = s.Users.Select(u => new UserBasicDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Role = u.Role,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ShipDetailDto?> GetShipWithUsersAsync(int id)
        {
            return await context.Ships
                .Where(s => s.Id == id)
                .Select(s => new ShipDetailDto
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    AssignedUsers = s.Users.Select(u => new UserDetailDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Role = u.Role,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync()
        {
            return await context.Ships
                .Where(s => !s.Users.Any())
                .Select(s => new ShipBasicDto
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                })
                .ToListAsync();
        }

        public async Task UpdateShipVelocityAsync(string shipCode, UpdateShipVelocityRequest request)
        {
            var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }
            ship.Velocity = request.Velocity;
            context.Ships.Update(ship);
            await context.SaveChangesAsync();
        }

        public async Task<ShipClosestPortResponse> GetClosestPortAsync(string shipCode)
        {
            var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }

            var ports = await context.Ports.ToListAsync();
            if (ports.Count == 0)
            {
                throw new KeyNotFoundException("No ports found in the system.");
            }

            var closestPort = ports
                .Select(port => new PortWithDistanceDto
                {
                    Port = port,
                    DistanceKm = DistanceCalculator.CalculateDistanceKm(
                        ship.Latitude, ship.Longitude,
                        port.Latitude, port.Longitude)
                })
                .OrderBy(p => p.DistanceKm)
                .First();

            var velocityKmh = (double)ship.Velocity * 1.852; // 1 knot = 1.852 km/h
            var estimatedHours = closestPort.DistanceKm / velocityKmh;
            var estimatedArrival = DateTime.UtcNow.AddHours(estimatedHours);

            return new ShipClosestPortResponse
            {
                ShipCode = ship.ShipCode,
                ShipName = ship.Name,
                PortName = closestPort?.Port?.Name ?? string.Empty,
                PortCountry = closestPort?.Port?.Country ?? string.Empty,
                DistanceToPort = closestPort?.DistanceKm ?? 0,
                EstimatedArrivalTime = estimatedArrival,
            };
        }

        public async Task<bool> DeleteShipAsync(int id)
        {
            var ship = await context.Ships.FindAsync(id);
            if (ship == null) return false;

            context.Ships.Remove(ship);
            await context.SaveChangesAsync();
            return true;
        }

    }
}