using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IShipService
    {
        Task<IEnumerable<ShipResponse>> GetShipsAsync();
        Task<ShipResponse> CreateShipAsync(CreateShipRequest newShip);
        Task<IEnumerable<AssignUsersToShipResponse>> AssignUsersToShipAsync(List<int> userIds, string shipCode);
        Task UnassignUsersFromShipAsync(List<int> userId, string shipCode);
        Task<ShipDetailDtoWithBasicUsers?> GetShipByCodeAsync(string shipCode);
        Task<ShipDetailDto?> GetShipWithUsersAsync(int id);
        Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync();
        Task<ShipBasicDto> UpdateShipAsync(string shipCode, Ship ship);
        Task<bool> DeleteShipAsync(int id);
    }
}