using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<UserDetailDto>> GetUsersDtoAsync();
        Task<UserDetailDto?> GetUserDtoByIdAsync(int id);
        Task<UserBasicDto?> GetUserWithShipsAsync(int id);
    }
}