using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<UserDto>> GetUsersDtoAsync();
        Task<UserDto?> GetUserDtoByIdAsync(int id);
        Task<UserDto?> GetUserWithShipsAsync(int id);
    }
}