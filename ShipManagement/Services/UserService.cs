using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Services
{
    public class UserService : IUserService
    {
        private readonly ShipManagementContext _context;

        public UserService(ShipManagementContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<UserDetailDto>> GetUsersDtoAsync()
        {
            return await _context.Users
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AssignedShips = u.UserShips
                    .Where(us => us.UserId == u.Id)
                    .Select(us => new ShipBasicDto
                    {
                        Id = us.Ship.Id,
                        ShipCode = us.Ship.ShipCode,
                        Name = us.Ship.Name,
                        Velocity = us.Ship.Velocity,
                        Latitude = us.Ship.Latitude,
                        Longitude = us.Ship.Longitude
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<UserDetailDto?> GetUserDtoByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AssignedShips = u.UserShips
                    .Where(us => us.UserId == u.Id)
                    .Select(us => new ShipBasicDto
                    {
                        Id = us.Ship.Id,
                        ShipCode = us.Ship.ShipCode,
                        Name = us.Ship.Name,
                        Velocity = us.Ship.Velocity,
                        Latitude = us.Ship.Latitude,
                        Longitude = us.Ship.Longitude
                    }).ToList()

                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserBasicDto?> GetUserWithShipsAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AssignedShips = u.UserShips.Select(us => new ShipBasicDto
                    {
                        Id = us.Ship.Id,
                        ShipCode = us.Ship.ShipCode,
                        Name = us.Ship.Name,
                        Velocity = us.Ship.Velocity,
                        Latitude = us.Ship.Latitude,
                        Longitude = us.Ship.Longitude
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}