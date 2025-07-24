namespace ShipManagement.Services
{
    public class UserService(ShipManagementContext context) : IUserService
    {
        public async Task<CreateUserResponse> CreateUserAsync(UserDetailsRequest request)
        {
            if (await context.Users.AnyAsync(u => u.Name == request.Name))
            {
                throw new DuplicateUserNameException(string.Format(Constants.Messages.User.DUPLICATE_NAME, request.Name));
            }

            var user = new User
            {
                Name = request.Name,
                Role = request.Role
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new CreateUserResponse
            {
                Id = user.Id
            };
        }

        public async Task<IEnumerable<GetUserResponse>> GetUsersAsync()
        {
            return await context.Users
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AssignedShips = u.Ships.Select(s => new ShipResponse
                    {
                        Id = s.Id,
                        ShipCode = s.ShipCode,
                        Name = s.Name,
                        Velocity = s.Velocity,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetUserResponse?> GetUserByIdAsync(int id)
        {
            return await context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AssignedShips = u.Ships.Select(s => new ShipResponse
                    {
                        Id = s.Id,
                        ShipCode = s.ShipCode,
                        Name = s.Name,
                        Velocity = s.Velocity,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<GetUserResponse> AssignShipsToUserSync(int userId, List<string> shipCodes)
        {
            var user = await context.Users.Include(u => u.Ships).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException(string.Format(Constants.Messages.UserShip.NOT_ASSIGNED, userId, "N/A"));
            }

            foreach (var shipCode in shipCodes)
            {
                var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
                if (ship != null && !user.Ships.Any(s => s.Id == ship.Id))
                {
                    user.Ships.Add(ship);
                }
            }

            await context.SaveChangesAsync();

            return new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                AssignedShips = user.Ships.Select(s => new ShipResponse
                {
                    Id = s.Id,
                    ShipCode = s.ShipCode,
                    Name = s.Name,
                    Velocity = s.Velocity,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }).ToList()
            };
        }

        public async Task UnassignShipsFromUserAsync(int userId, List<string> shipCodes)
        {
            var user = await context.Users.Include(u => u.Ships).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException(string.Format(Constants.Messages.UserShip.NOT_ASSIGNED, userId, "N/A"));
            }

            foreach (var shipCode in shipCodes)
            {
                var ship = await context.Ships.FirstOrDefaultAsync(s => s.ShipCode == shipCode);
                if (ship == null)
                {
                    throw new KeyNotFoundException(string.Format(Constants.Messages.Ship.NOT_FOUND, shipCode));
                }
                if (user.Ships.Any(s => s.Id == ship.Id))
                {
                    user.Ships.Remove(ship);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return false;
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;

        }
    }
}