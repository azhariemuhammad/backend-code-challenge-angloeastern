using Microsoft.AspNetCore.Mvc;
using ShipManagement.Interfaces;
using ShipManagement.Models;
using ShipManagement.Models.Attributes;
using ShipManagement.Models.DTOs;
using ShipManagement.Services;
using ShipManagement.Constants;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
            Summary = "Retrieves all ships.",
            Description = "Returns a list of all ships in the system."
        )]
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
        [SwaggerOperation(
            Summary = "Retrieves a ship by its code.",
            Description = "Returns ship details for the specified ship code. Returns 404 if not found."
        )]
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
        [SwaggerOperation(
            Summary = "Creates a new ship.",
            Description = "Adds a new ship to the system. Returns the created ship. Fails if the ship code already exists."
        )]
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
        [SwaggerOperation(
            Summary = "Assigns a user to a ship.",
            Description = "Assigns the specified user to the specified ship. Returns the assignment details."
        )]
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
        [SwaggerOperation(
            Summary = "Unassigns a user from a ship.",
            Description = "Removes the assignment of the specified user from the specified ship. Returns the updated ship."
        )]
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
        [SwaggerOperation(
            Summary = "Updates a ship or updates the velocity of a ship.",
            Description = "Updates the details of the specified ship. Returns the updated ship. Fails if the ship code is not found or duplicate."
        )]
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
        [SwaggerOperation(
            Summary = "Retrieves all unassigned ships.",
            Description = "Returns a list of ships that are not assigned to any user."
        )]
        public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetUnAssignedShips()
        {
            try
            {
                const string cacheKey = "unassigned_ships";
                var cachedResponse = await _redisCacheService.GetAsync<IEnumerable<ShipBasicDto>>(cacheKey);

                if (cachedResponse is not null)
                {
                    return Ok(cachedResponse);
                }

                var ships = await _shipService.GetUnAssignedShipsAsync();
                await _redisCacheService.SetAsync(cacheKey, ships, TimeSpan.FromMinutes(5));

                return Ok(ships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = Messages.Ship.RetrieveUnassignedError, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a ship.",
            Description = "Deletes the ship with the specified ID. Returns 204 No Content if successful, 404 if not found."
        )]
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