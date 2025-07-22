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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserRequestDto registerUserRequestDto)
        {
            var serverResponse = new ServerResponse<UserDto>();
            try
            {
                serverResponse = await _authService.RegisterUserAsync(registerUserRequestDto);
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
                var response = await _authService.LoginAsync(loginDto);
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                var response = await _authService.SendPasswordResetTokenAsync(request);
                if (!response.Success)
                    return BadRequest(response);

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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                var response = await _authService.ResetPasswordAsync(request);
                if (!response.Success)
                    return BadRequest(response);
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

    }
}

