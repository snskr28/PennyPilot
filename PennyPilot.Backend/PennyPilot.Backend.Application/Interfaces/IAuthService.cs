using PennyPilot.Backend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServerResponse<UserDto>> RegisterUserAsync(RegisterUserRequestDto requestDto);
        Task<ServerResponse<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto);
        Task<bool> SendPasswordResetTokenAsync(ForgotPasswordRequestDto request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}
