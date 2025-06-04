using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityService _securityService;
        private readonly IEmailService _emailService;
        public AuthService(IUnitOfWork unitOfWork, ISecurityService securityService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _securityService = securityService;
            _emailService = emailService;
        }

        public async Task<ServerResponse<UserDto>> RegisterUserAsync(RegisterUserRequestDto requestDto)
        {
            var existingUserByEmail = await _unitOfWork.Users.GetUserByEmailAsync(requestDto.Email);
            var existingUserByUsername = await _unitOfWork.Users.GetUserByUsernameAsync(requestDto.Username);
            if (existingUserByEmail != null)
            {
                return new ServerResponse<UserDto>
                {
                    Success = false,
                    Message = "Email already in use."
                };
            }
            if (existingUserByUsername != null)
            {
                return new ServerResponse<UserDto>
                {
                    Success = false,
                    Message = "Username already in use."
                };
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = requestDto.Username,
                FirstName = requestDto.FirstName,
                MiddleName = requestDto.MiddleName,
                LastName = requestDto.LastName,
                Dob = requestDto.DOB,
                Email = requestDto.Email,
                PasswordHash = _securityService.HashPassword(requestDto.Password), 
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false
            };

            var createdUser = await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserDto
            {
                Username = createdUser.Username,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                DOB = createdUser.Dob,
                CreatedAt = createdUser.CreatedAt
            };

            return new ServerResponse<UserDto>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = userDto
            };
        }
        public async Task<ServerResponse<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(requestDto.Identifier)
                           ?? await _unitOfWork.Users.GetUserByUsernameAsync(requestDto.Identifier);

                if (user == null || !_securityService.VerifyPassword(requestDto.Password, user.PasswordHash))
                {
                    return new ServerResponse<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Invalid credentials."
                    };
                }

                var token = _securityService.GenerateJwtToken(user);

                return new ServerResponse<LoginResponseDto>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = new LoginResponseDto
                    {
                        Token = token,
                        Username = user.Username,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendPasswordResetTokenAsync(ForgotPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Identifier);
            if (user == null)
                return false; // optionally do not disclose if email exists or not

            // Generate token (secure random string)
            var token = _securityService.GenerateSecureToken();

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Compose reset link
            var resetLink = $"https://yourfrontend.com/reset-password?token={token}";

            var emailBody = $"Click this link to reset your password: {resetLink}\n\nThis link expires in 1 hour.";

            await _emailService.SendEmailAsync(user.Email, "PennyPilot Password Reset", emailBody);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(request.Token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return false;

            // Hash new password - (replace with your password hasher)
            user.PasswordHash = _securityService.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);

            return true;
        }

    }
}
