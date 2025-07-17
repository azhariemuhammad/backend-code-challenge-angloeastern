using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models.DTOs;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    // [Authorize]
    public class PortController : ControllerBase
    {
        private readonly IPortService _portService;

        public PortController(IPortService portService)
        {
            _portService = portService;
        }

        [HttpGet("closest-port/{shipId}")]
        public async Task<ActionResult<PortWithDistanceDto>> GetClosestPort(int shipId)
        {
            var closestPort = await _portService.GetClosestPortAsync(shipId);
            return Ok(closestPort);
        }

        [HttpGet("estimated-arrival/{shipId}")]
        public async Task<ActionResult<EstimatedArrivalDto>> GetEstimatedArrival(int shipId)
        {
            var estimatedArrival = await _portService.GetEstimatedArrivalAsync(shipId);
            return Ok(estimatedArrival);
        }
    }
}