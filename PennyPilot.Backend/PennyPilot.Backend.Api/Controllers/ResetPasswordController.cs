using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Services;

namespace PennyPilot.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly ForgotPasswordService _forgotPasswordService;

        public ResetPasswordController(ForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var result = await _forgotPasswordService.SendPasswordResetTokenAsync(request);
            if (!result)
                return BadRequest("Email not found.");

            return Ok("Password reset link sent if email exists.");
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
