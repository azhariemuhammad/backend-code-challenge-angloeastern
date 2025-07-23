namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves all users.",
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