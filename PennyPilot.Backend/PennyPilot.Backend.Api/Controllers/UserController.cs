using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;

namespace PennyPilot.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<UserDto>> GetUserById(int id)
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                    return NotFound();

                return Ok(user);
            }

            [HttpGet("active")]
            public async Task<ActionResult<IEnumerable<UserDto>>> GetActiveUsers()
            {
                var users = await _userService.GetActiveUsersAsync();
                return Ok(users);
            }

            [HttpGet("byemail")]
            public async Task<ActionResult<UserDto>> GetUserByEmail([FromQuery] string email)
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                    return NotFound();

                return Ok(user);
            }

            [HttpPost]
            public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
            {
                var createdUser = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
            {
                try
                {
                    await _userService.UpdateUserAsync(id, updateUserDto);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                try
                {
                    await _userService.DeleteUserAsync(id);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
        }
    }
}
