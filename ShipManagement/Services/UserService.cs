namespace ShipManagement.Services
{
    public class UserService(ShipManagementContext context) : IUserService
    {
        public async Task<CreateUserResponse> CreateUserAsync(UserDetailsRequest request)
        {
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
                    AssignedShips = u.UserShips
                    .Where(us => us.UserId == u.Id)
                    .Select(us => new ShipResponse
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
                    AssignedShips = u.UserShips
                        .Where(us => us.UserId == u.Id)
                        .Select(us => new ShipResponse
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