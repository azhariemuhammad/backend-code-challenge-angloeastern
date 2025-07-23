

using ShipManagement.Constants;
using ShipManagement.Models.Attributes;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    // [Authorize]
    public class ShipController(IShipService userShipService, IRedisCacheService redisCacheService) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves all ships.",
            Description = "Returns a list of all ships in the system."
        )]
        [ProducesResponseType(typeof(IEnumerable<ShipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ShipResponse>>> GetShipsAsync()
        {
            var ships = await userShipService.GetShipsAsync();
            return Ok(ships);
        }

        [HttpGet("{shipCode}")]
        [SwaggerOperation(
            Summary = "Retrieves a ship by its code.",
            Description = "Returns ship details for the specified ship code. Returns 404 if not found."
        )]
        [ProducesResponseType(typeof(ShipBasicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ShipCodeFilter]
        public async Task<ActionResult<ShipResponse>> GetShipByCodeAsync([FromRoute] string shipCode)
        {
            var ship = await userShipService.GetShipByCodeAsync(shipCode);
            if (ship == null)
            {
                return NotFound();
            }
            return Ok(ship);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new ship.",
            Description = "Adds a new ship to the system. Returns the created ship. Fails if the ship code already exists."
        )]
        [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Ship>> CreateShip([FromBody] CreateShipRequest newShip)
        {
            var createdShip = await userShipService.CreateShipAsync(newShip);
            return Ok(createdShip);
        }

        [HttpPost("{shipCode}/assign")]
        [SwaggerOperation(
            Summary = "Assigns users to a ship.",
            Description = "Assigns the specified user to the specified ship. Returns the assignment details."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<AssignUsersToShipResponse>>> AssignUsersToShipAsync(
            [FromBody] List<int> userIds,
            [FromRoute][ValidShipCode] string shipCode)
        {
            var assignedUsers = await userShipService.AssignUsersToShipAsync(userIds, shipCode);
            return Ok(assignedUsers);
        }

        [HttpPut("{shipCode}/unassign")]
        [SwaggerOperation(
            Summary = "Unassigns users from a ship.",
            Description = "Removes the assignment of the specified user from the specified ship. Returns the updated ship."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnassignUsersFromShipAsync(
            [FromBody] List<int> userIds,
            [FromRoute][ValidShipCode] string shipCode)
        {
            await userShipService.UnassignUsersFromShipAsync(userIds, shipCode);
            return Ok(new { message = "Users unassigned successfully." });
        }

        // [HttpPut("{shipCode}")]
        // [RequiredValidShipCode]
        // [SwaggerOperation(
        //     Summary = "Updates a ship or updates the velocity of a ship.",
        //     Description = "Updates the details of the specified ship. Returns the updated ship. Fails if the ship code is not found or duplicate."
        // )]
        // [ProducesResponseType(typeof(ShipBasicDto), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status409Conflict)]
        // public async Task<ActionResult<ShipBasicDto>> UpdateShip(string shipCode, [FromBody] ShipBasicDto shipDto)
        // {
        //     var updatedShip = await userShipService.UpdateShipAsync(shipCode, shipDto);
        //     if (updatedShip == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(updatedShip);
        // }

        // [HttpGet]
        // [Route("unassigned")]
        // [SwaggerOperation(
        //     Summary = "Retrieves all unassigned ships.",
        //     Description = "Returns a list of ships that are not assigned to any user."
        // )]
        // [ProducesResponseType(typeof(IEnumerable<ShipBasicDto>), StatusCodes.Status200OK)]
        // public async Task<ActionResult<IEnumerable<ShipBasicDto>>> GetUnAssignedShips()
        // {
        //     const string cacheKey = "unassigned_ships";
        //     var cachedResponse = await redisCacheService.GetAsync<IEnumerable<ShipBasicDto>>(cacheKey);
        //     if (cachedResponse is not null)
        //     {
        //         return Ok(cachedResponse);
        //     }
        //     var ships = await userShipService.GetUnAssignedShipsAsync();
        //     await redisCacheService.SetAsync(cacheKey, ships, TimeSpan.FromMinutes(5));
        //     return Ok(ships);
        // }

        // [HttpDelete("{shipCode}")]
        // [SwaggerOperation(
        //     Summary = "Deletes a ship.",
        //     Description = "Deletes the ship with the specified code. Returns 204 No Content if successful, 404 if not found."
        // )]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> DeleteShip(string shipCode)
        // {
        //     var result = await userShipService.DeleteShipAsync(shipCode);
        //     if (!result)
        //     {
        //         return NotFound();
        //     }
        //     return NoContent();
        // }
    }
}