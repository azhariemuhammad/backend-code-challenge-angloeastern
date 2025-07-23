using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateUserAsync(UserDetailsRequest request);
        Task<IEnumerable<GetUserResponse>> GetUsersAsync();
        Task<GetUserResponse?> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}