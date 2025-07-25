using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IShipService
    {
        Task<IEnumerable<ShipResponse>> GetShipsAsync();
        Task<ShipResponse> CreateShipAsync(CreateShipRequest newShip);
        Task<ShipDetails?> GetShipByCodeAsync(string shipCode);
        Task<IEnumerable<ShipResponse>> GetUnassignedShipAsync();
        Task<IEnumerable<ShipBasicDto>> GetUnAssignedShipsAsync();
        Task UpdateShipVelocityAsync(string shipCode, UpdateShipVelocityRequest request);
        Task<ShipClosestPortResponse> GetClosestPortAsync(string shipCode);
        Task<bool> DeleteShipAsync(int id);
    }
}