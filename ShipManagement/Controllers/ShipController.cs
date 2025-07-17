using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;
using ShipManagement.Services;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    // [Authorize]
    public class ShipController : ControllerBase
    {
        private readonly IShipService _shipService;

        public ShipController(IShipService userShipService)
        {
            _shipService = userShipService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetShips()
        {
            var ships = await _shipService.GetShipsAsync();
            return Ok(ships);
        }

        [HttpGet("{shipCode}")]
        [RequiredValidShipCode]
        public async Task<ActionResult<ShipBasicDto>> GetShipByCode(string shipCode)
        {
            var ship = await _shipService.GetShipByCodeAsync(shipCode);
            if (ship == null)
            {
                return NotFound();
            }
            return Ok(ship);
        }

        [HttpPost]
        public async Task<ActionResult<Ship>> CreateShip(Ship ship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdShip = await _shipService.CreateShipAsync(ship);
            return Ok(createdShip);
        }

        [HttpPost]
        [Route("assign/{shipId}")]
        public async Task<ActionResult<UserShip>> CreateUserShip(int userId, int shipId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _shipService.AssignedUser(userId, shipId);

            return Ok(new { UserId = userId, ShipId = shipId });
        }

        [HttpPut]
        [Route("unassigned/{shipId}")]
        public async Task<ActionResult<ShipBasicDto>> UnassigneUserShip(int userId, int shipId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedShip = await _shipService.UnassignedUserShipAsync(userId, shipId);
            if (updatedShip == null)
            {
                return NotFound();
            }

            return Ok(updatedShip);
        }

        [HttpPut]
        [Route("{shipCode}")]
        [RequiredValidShipCode]
        public async Task<ActionResult<ShipBasicDto>> UpdateShip(string shipCode, Ship ship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedShip = await _shipService.UpdateShipAsync(shipCode, ship);
            if (updatedShip == null)
            {
                return NotFound();
            }

            return Ok(updatedShip);
        }

        [HttpGet]
        [Route("unassigned")]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetUnAssignedShips()
        {
            var ships = await _shipService.GetUnAssignedShipsAsync();

            return Ok(ships);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShip(int id)
        {
            var result = await _shipService.DeleteShipAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}