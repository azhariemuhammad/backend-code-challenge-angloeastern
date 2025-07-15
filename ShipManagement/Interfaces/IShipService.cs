using ShipManagement.Models;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IShipService
    {
        Task<Ship> CreateShipAsync(Ship ship);
        Task<UserShip> AssignedUser(int userId, int shipId);
        Task<IEnumerable<ShipBasicDto>> GetShipsAsync();
        Task<ShipBasicDto?> GetShipByIdAsync(int id);
        Task<ShipDetailDto?> GetShipWithUsersAsync(int id);
        Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync();
    }
}