using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Services
{
    public class ShipService : IShipService
    {
        private readonly ShipManagementContext _context;

        public ShipService(ShipManagementContext context)
        {
            _context = context;
        }

        public async Task<Ship> CreateShipAsync(Ship ship)
        {
            _context.Ships.Add(ship);
            await _context.SaveChangesAsync();
            return ship;
        }

        public async Task<UserShip> AssignedUser(int userId, int shipId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null)
            {
                throw new KeyNotFoundException($"Ship with ID {shipId} not found.");
            }
            var newUserShip = new UserShip
            {
                UserId = userId,
                ShipId = shipId,
                CreatedAt = DateTime.UtcNow,
                User = user,
                Ship = ship
            };

            _context.UserShips.Add(newUserShip);
            await _context.SaveChangesAsync();

            return newUserShip;
        }

        public async Task<IEnumerable<ShipBasicDto>> GetShipsAsync()
        {
            return await _context.Ships
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

        public async Task<ShipBasicDto?> GetShipByIdAsync(int id)
        {
            return await _context.Ships
                .Where(s => s.Id == id)
                .Select(s => new ShipBasicDto
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ShipDetailDto?> GetShipWithUsersAsync(int id)
        {
            return await _context.Ships
                .Where(s => s.Id == id)
                .Select(s => new ShipDetailDto
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    AssignedUsers = s.UserShips.Select(us => new UserDto
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
            return await _context.Ships
                .Where(s => !_context.UserShips.Any(us => us.ShipId == s.Id))
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
            var existingShip = await _context.Ships.Where(s => s.ShipCode == shipCode).FirstOrDefaultAsync();
            if (existingShip == null)
            {
                throw new KeyNotFoundException($"Ship with code {shipCode} not found.");
            }

            existingShip.ShipCode = ship.ShipCode;
            existingShip.Name = ship.Name;
            existingShip.Velocity = ship.Velocity;
            existingShip.Latitude = ship.Latitude;
            existingShip.Longitude = ship.Longitude;

            _context.Ships.Update(existingShip);
            await _context.SaveChangesAsync();

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
    }

}