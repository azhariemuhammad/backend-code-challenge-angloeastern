
using Microsoft.AspNetCore.Http.HttpResults;

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
            var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }
            var assignedShips = new List<AssignUsersToShipResponse>();
            foreach (var user in users)
            {
                var userShip = new UserShip
                {
                    UserId = user.Id,
                    ShipId = ship.Id
                };
                context.UserShips.Add(userShip);
                assignedShips.Add(new AssignUsersToShipResponse
                {
                    UserId = user.Id,
                    ShipCode = ship.ShipCode,
                    ShipName = ship.Name
                });
            }
            await context.SaveChangesAsync();
            return assignedShips;
        }

        public async Task UnassignUsersFromShipAsync(List<int> userIds, string shipCode)
        {
            var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }
            var userShips = await context.UserShips
                .Where(us => userIds.Contains(us.UserId) && us.ShipId == ship.Id)
                .ToListAsync();
            if (userShips.Count == 0)
            {
                throw new KeyNotFoundException("No user-ship assignments found for the provided user IDs and ship code.");
            }
            context.UserShips.RemoveRange(userShips);
            await context.SaveChangesAsync();
        }


        public async Task<ShipDetailDtoWithBasicUsers?> GetShipByCodeAsync(string shipCode)
        {
            return await context.Ships
                .Where(s => s.ShipCode == shipCode)
                .Select(s => new ShipDetailDtoWithBasicUsers
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    AssignedUsers = s.UserShips.Select(us => new UserBasicDto
                    {
                        Id = us.User.Id,
                        Name = us.User.Name,
                        Role = us.User.Role,
                        CreatedAt = us.User.CreatedAt,
                        UpdatedAt = us.User.UpdatedAt
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
                    AssignedUsers = s.UserShips.Select(us => new UserDetailDto
                    {
                        Id = us.User.Id,
                        Name = us.User.Name,
                        Role = us.User.Role,
                        CreatedAt = us.User.CreatedAt,
                        UpdatedAt = us.User.UpdatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync()
        {
            return await context.Ships
                .Where(s => !context.UserShips.Any(us => us.ShipId == s.Id))
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

        public async Task<ShipBasicDto> UpdateShipAsync(string shipCode, Ship ship)
        {
            var existingShip = await context.Ships.Where(s => s.ShipCode == shipCode).FirstOrDefaultAsync();
            if (existingShip == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }

            existingShip.ShipCode = ship.ShipCode;
            existingShip.Name = ship.Name;
            existingShip.Velocity = ship.Velocity;
            existingShip.Latitude = ship.Latitude;
            existingShip.Longitude = ship.Longitude;

            context.Ships.Update(existingShip);
            await context.SaveChangesAsync();

            return new ShipBasicDto
            {
                Id = existingShip.Id,
                ShipCode = existingShip.ShipCode,
                Name = existingShip.Name,
                Velocity = existingShip.Velocity,
                Latitude = existingShip.Latitude,
                Longitude = existingShip.Longitude
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