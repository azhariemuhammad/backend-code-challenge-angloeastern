using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<UserDetailDto>> GetUsersAsync();
        Task<UserDetailDto?> GetUserByIdAsync(int id);
        Task<UserBasicDto?> GetUserWithShipsAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}