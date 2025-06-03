using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Api.Helpers;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Services;
using System.Security.Claims;

namespace PennyPilot.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ForgotPasswordService _forgotPasswordService;


        public AuthController(IUserService userService, ForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserRequestDto registerUserRequestDto)
        {
            var serverResponse = new ServerResponse<UserDto>();
            try
            {
                serverResponse = await _userService.RegisterUserAsync(registerUserRequestDto);
                if (!serverResponse.Success)
                    return BadRequest(serverResponse);

                return Ok(serverResponse);
            }
            catch (Exception ex)
            {
                serverResponse.Data = null;
                serverResponse.Success = false;
                serverResponse.Message = ex.Message;
                return StatusCode(500, serverResponse);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                var response = await _userService.LoginAsync(loginDto);
                if (!response.Success)
                    return Unauthorized(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServerResponse<string>
                {
                    Success = false,
                    Message = "Internal server error: " + ex.Message
                });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();

            if (userId == Guid.Empty)
                return Unauthorized(new ServerResponse<string>
                {
                    Success = false,
                    Message = "Invalid user token."
                });

            var response = await _userService.GetCurrentUserProfileAsync(userId);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var result = await _forgotPasswordService.SendPasswordResetTokenAsync(request);
            if (!result)
                return BadRequest("Email not found.");

            return Ok("Password reset link sent");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var result = await _forgotPasswordService.ResetPasswordAsync(request);
            if (!result)
                return BadRequest("Invalid or expired token.");

            return Ok("Password reset successfully.");
        }

    }
}

