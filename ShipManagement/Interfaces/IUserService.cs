using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateUserAsync(UserDetailsRequest request);
        Task<IEnumerable<GetUserResponse>> GetUsersAsync();
        Task<GetUserResponse?> GetUserByIdAsync(int id);
        Task<GetUserResponse> AssignShipsToUserSync(int userId, List<string> shipCodes);
        Task UnassignShipsFromUserAsync(int userId, List<string> shipCodes);
        Task<bool> DeleteUserAsync(int id);
    }
}