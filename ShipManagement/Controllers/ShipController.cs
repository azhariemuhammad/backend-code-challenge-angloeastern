using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.DTOs;
using ShipManagement.Services;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    // [Authorize]
    public class ShipController : ControllerBase
    {
        private readonly IShipService _shipSerivce;

        public ShipController(IShipService userShipService)
        {
            _shipSerivce = userShipService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetShips()
        {
            var ships = await _shipSerivce.GetShipsAsync();
            return Ok(ships);
        }

        [HttpPost]
        public async Task<ActionResult<Ship>> CreateShip(Ship ship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdShip = await _shipSerivce.CreateShipAsync(ship);
            return Ok(createdShip);
        }

        [HttpPost]
        [Route("assign")]
        public async Task<ActionResult<UserShip>> CreateUserShip(int userId, int shipId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _shipSerivce.AssignedUser(userId, shipId);

            return Ok(new { UserId = userId, ShipId = shipId });
        }

        [HttpGet]
        [Route("unassigned-list")]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetUnAssignedShips()
        {
            var ships = await _shipSerivce.GetUnAssignedShipsAsync();

            return Ok(ships);
        }
    }
}