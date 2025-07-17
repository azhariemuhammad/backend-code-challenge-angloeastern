using ShipManagement.Models.DTOs;

namespace ShipManagement.Interfaces
{
    public interface IPortService
    {
        Task<PortWithDistanceDto> GetClosestPortAsync(int shipId);
        Task<EstimatedArrivalDto> GetEstimatedArrivalAsync(int shipId);
    }

}