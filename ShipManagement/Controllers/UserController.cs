namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves all users with their assigned ships.",
            Description = "Returns a list of all users in the system."
        )]
        [ProducesResponseType(typeof(IEnumerable<GetUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetUsersAsync()
        {
            var users = await userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("{id}/ships/assign")]
        [SwaggerOperation(
            Summary = "Updates ships assigned to a user.",
            Description = "Updates the list of ships assigned to the specified user. Replaces the user's ship assignments with the provided list."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignShipsToUserSync(
            int id,
            [FromBody][ValidShipCodes][SwaggerParameter(Description = "A list of valid ship codes to assign to the user. Each code must correspond to an existing ship.")] List<string> shipCodes)
        {
            await userService.AssignShipsToUserSync(id, shipCodes);
            return Ok(new { message = "User's ships assigned successfully." });

        }

        [HttpPost("{id}/ships/unassign")]
        [SwaggerOperation(
            Summary = "Unassigns ships from a user.",
            Description = "Removes ship assignments for the specified user."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignShipsFromUserAsync(
            int id,
            [FromBody][ValidShipCodes][SwaggerParameter(Description = "A list of valid ship codes to unassign from the user. Each code must correspond to an existing ship.")] List<string> shipCodes)
        {
            await userService.UnassignShipsFromUserAsync(id, shipCodes);
            return Ok(new { message = "User's ships unassigned successfully." });
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new user.",
            Description = "Adds a new user to the system and returns the created user."
        )]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateUserResponse>> CreateUserAsync(
            [FromBody] UserDetailsRequest request)
        {
            var result = await userService.CreateUserAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves a user by ID.",
            Description = "Returns user details for the specified user ID. Returns 404 if not found."
        )]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> GetUserByIdAsync(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a user.",
            Description = "Deletes the user with the specified ID. Returns 204 No Content if successful, 404 if not found."
        )]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}