using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IShipService
    {
        Task<Ship> CreateShipAsync(Ship ship);
        Task<UserShip> AssignedUser(int userId, int shipId);
        Task<UserShip> UnassignedUserShipAsync(int userId, int shipId);
        Task<IEnumerable<ShipBasicDto>> GetShipsAsync();
        Task<ShipDetailDtoWithBasicUsers?> GetShipByCodeAsync(string shipCode);
        Task<ShipDetailDto?> GetShipWithUsersAsync(int id);
        Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync();
        Task<ShipBasicDto> UpdateShipAsync(string shipCode, Ship ship);
    }
}