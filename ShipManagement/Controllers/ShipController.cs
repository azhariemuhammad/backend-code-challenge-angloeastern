using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;
using ShipManagement.Services;
using ShipManagement.Constants;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    // [Authorize]
    public class ShipController : ControllerBase
    {
        private readonly IShipService _shipService;
        private readonly IRedisCacheService _redisCacheService;

        public ShipController(IShipService userShipService, IRedisCacheService redisCacheService)
        {
            _shipService = userShipService;
            _redisCacheService = redisCacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetShips()
        {
            try
            {
                var ships = await _shipService.GetShipsAsync();
                return Ok(ships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.RetrieveAllError, error = ex.Message });
            }
        }

        [HttpGet("{shipCode}")]
        [RequiredValidShipCode]
        public async Task<ActionResult<ShipBasicDto>> GetShipByCode(string shipCode)
        {
            try
            {
                var ship = await _shipService.GetShipByCodeAsync(shipCode);
                if (ship == null)
                {
                    return NotFound(new { message = string.Format(Messages.Ship.NotFound, shipCode) });
                }
                return Ok(ship);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.RetrieveError, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Ship>> CreateShip(Ship ship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if ship code already exists
                var existingShip = await _shipService.GetShipByCodeAsync(ship.ShipCode);
                if (existingShip != null)
                {
                    return Conflict(new { message = string.Format(Messages.Ship.DuplicateShipCode, ship.ShipCode) });
                }

                var createdShip = await _shipService.CreateShipAsync(ship);
                return CreatedAtAction(nameof(GetShipByCode), new { shipCode = createdShip.ShipCode }, createdShip);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("duplicate key") || ex.Message.Contains("unique constraint"))
            {
                return Conflict(new { message = string.Format(Messages.Ship.DuplicateShipCode, ship.ShipCode) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.CreateError, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("assign/{shipId}")]
        public async Task<ActionResult<UserShip>> CreateUserShip(int userId, int shipId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _shipService.AssignedUser(userId, shipId);
                return Ok(new { UserId = userId, ShipId = shipId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.UserShip.AssignError, error = ex.Message });
            }
        }

        [HttpPut]
        [Route("unassigned/{shipId}")]
        public async Task<ActionResult<ShipBasicDto>> UnassigneUserShip(int userId, int shipId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedShip = await _shipService.UnassignedUserShipAsync(userId, shipId);
                if (updatedShip == null)
                {
                    return NotFound(new { message = string.Format(Messages.UserShip.NotAssigned, userId, shipId) });
                }

                return Ok(updatedShip);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.UserShip.UnassignError, error = ex.Message });
            }
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

            try
            {
                var updatedShip = await _shipService.UpdateShipAsync(shipCode, ship);
                if (updatedShip == null)
                {
                    return NotFound(new { message = string.Format(Messages.Ship.NotFound, shipCode) });
                }

                return Ok(updatedShip);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("duplicate key") || ex.Message.Contains("unique constraint"))
            {
                return Conflict(new { message = string.Format(Messages.Ship.DuplicateShipCode, ship.ShipCode) });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.UpdateError, error = ex.Message });
            }
        }

        [HttpGet]
        [Route("unassigned")]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetUnAssignedShips()
        {
            try
            {
                // const string cacheKey = "unassigned_ships";
                // var cachedResponse = await _redisCacheService.GetAsync<IEnumerable<ShipBasicDto>>(cacheKey);

                // if (cachedResponse is not null)
                // {
                //     return Ok(cachedResponse);
                // }

                var ships = await _shipService.GetUnAssignedShipsAsync();
                // await _redisCacheService.SetAsync(cacheKey, ships, TimeSpan.FromMinutes(5));

                return Ok(ships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.RetrieveUnassignedError, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShip(int id)
        {
            try
            {
                var result = await _shipService.DeleteShipAsync(id);
                if (!result)
                {
                    return NotFound(new { message = string.Format(Messages.Ship.NotFoundById, id) });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.DeleteError, error = ex.Message });
            }
        }
    }
}