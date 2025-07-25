
namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/ships")]
    public class ShipController(IShipService userShipService) : ControllerBase
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
        [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status200OK)]
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
        public async Task<ActionResult<Ship>> CreateShipAsync([FromBody] CreateShipRequest newShip)
        {
            var createdShip = await userShipService.CreateShipAsync(newShip);
            return Ok(createdShip);
        }

        [HttpPut("{shipCode}")]
        [SwaggerOperation(
            Summary = "Updates the velocity of a ship.",
            Description = "Updates the velocity of the specified ship. Returns a success message. Fails if the ship code is not found."
        )]
        [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ShipResponse>> UpdateShipVelocityAsync([ValidShipCode] string shipCode, [FromBody] UpdateShipVelocityRequest request)
        {
            await userShipService.UpdateShipVelocityAsync(shipCode, request);
            return Ok(new { message = "Ship velocity updated successfully." });
        }
        [HttpGet("ships/unassigned")]
        [SwaggerOperation(
            Summary = "Retrieves unassigned ships.",
            Description = "Returns ships that are not currently assigned to any user."
        )]
        [ProducesResponseType(typeof(IEnumerable<ShipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ShipResponse>>> GetUnassignedShipAsync()
        {
            var ships = await userShipService.GetUnassignedShipAsync();
            return Ok(ships);
        }

        [HttpGet("{shipCode}/closest-port")]
        [SwaggerOperation(
            Summary = "Retrieves the closest port for a ship.",
            Description = "Returns the closest port and distance for the specified ship code. Returns 404 if the ship is not found."
        )]
        [ProducesResponseType(typeof(ShipClosestPortResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShipClosestPortResponse>> GetClosestPortAsync([ValidShipCode] string shipCode)
        {
            var closestPort = await userShipService.GetClosestPortAsync(shipCode);
            return Ok(closestPort);
        }
    }
}