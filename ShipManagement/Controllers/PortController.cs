using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models.DTOs;
using ShipManagement.Constants;

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
            try
            {
                var closestPort = await _portService.GetClosestPortAsync(shipId);
                if (closestPort == null)
                {
                    return NotFound(new { message = string.Format(Messages.Port.ShipNotFoundForPort, shipId) });
                }
                return Ok(closestPort);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Port.ClosestPortError, error = ex.Message });
            }
        }

        [HttpGet("estimated-arrival/{shipId}")]
        public async Task<ActionResult<EstimatedArrivalDto>> GetEstimatedArrival(int shipId)
        {
            try
            {
                var estimatedArrival = await _portService.GetEstimatedArrivalAsync(shipId);
                if (estimatedArrival == null)
                {
                    return NotFound(new { message = string.Format(Messages.Port.ShipNotFoundForPort, shipId) });
                }
                return Ok(estimatedArrival);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Port.EstimatedArrivalError, error = ex.Message });
            }
        }
    }
}