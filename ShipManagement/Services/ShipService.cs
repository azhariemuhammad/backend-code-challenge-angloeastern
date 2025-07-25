

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
                throw new DuplicateShipCodeException(string.Format(Constants.Messages.Ship.DUPLICATE_SHIP_CODE, ship.ShipCode));
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
                throw new KeyNotFoundException(Constants.Messages.UserShip.NOT_ASSIGNED);
            }
            var ship = await context.Ships.Include(s => s.Users).FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException(string.Format(Constants.Messages.Ship.NOT_FOUND, shipCode));
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
                throw new KeyNotFoundException(string.Format(Constants.Messages.Ship.NOT_FOUND, shipCode));
            }
            var usersToRemove = ship.Users.Where(u => userIds.Contains(u.Id)).ToList();
            if (usersToRemove.Count == 0)
            {
                throw new KeyNotFoundException(Constants.Messages.UserShip.NOT_ASSIGNED);
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

        public async Task<IEnumerable<ShipResponse>> GetUnassignedShipAsync()
        {
            return await context.Ships
                .Where(s => !s.Users.Any())
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
                throw new KeyNotFoundException(string.Format(Constants.Messages.Ship.NOT_FOUND, shipCode));
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
                throw new KeyNotFoundException(string.Format(Constants.Messages.Ship.NOT_FOUND, shipCode));
            }

            var ports = await context.Ports.ToListAsync();
            if (ports.Count == 0)
            {
                throw new KeyNotFoundException(Constants.Messages.Port.NO_PORTS_AVAILABLE);
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
                ShipLatitude = ship.Latitude,
                ShipLongitude = ship.Longitude,
                ShipVelocity = ship.Velocity,
                PortName = closestPort.Port.Name,
                PortLatitude = closestPort.Port.Latitude,
                PortLongitude = closestPort.Port.Longitude,
                PortCountry = closestPort.Port.Country,
                DistanceToPort = closestPort.DistanceKm,
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