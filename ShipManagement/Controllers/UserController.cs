using ShipManagement.Models;
using ShipManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ShipManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    // [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieves all users.",
            Description = "Returns a list of all users in the system."
        )]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new user.",
            Description = "Adds a new user to the system and returns the created user."
        )]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves a user by ID.",
            Description = "Returns user details for the specified user ID. Returns 404 if not found."
        )]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
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
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}