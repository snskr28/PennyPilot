using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        public async Task<ServerResponse<string>> SendPasswordResetTokenAsync(ForgotPasswordRequestDto requestDto)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(requestDto.Identifier)
                        ?? await _unitOfWork.Users.GetUserByUsernameAsync(requestDto.Identifier); ;
            if (user == null)
                return new ServerResponse<string>
                {
                    Data = "Username/Email not found",
                    Success = false,
                    Message = "Username/Email not found"
                };

            // Generate token (secure random string)
            var token = _securityService.GenerateSecureToken();

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Compose reset link
            var resetLink = $"http://localhost:4200/reset-password?token={token}";

            var subject = "Reset Your PennyPilot Password";
            var emailBody = $@"
                                <html>
                                    <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <h2 style='color: #2c3e50;'>Password Reset Request</h2>
                                        <p>Hi {user.Username},</p>
                                        <p>We received a request to reset your password for your <strong>PennyPilot</strong> account.</p>
                                        <p>Click the button below to reset your password. This link will expire in <strong>1 hour</strong>.</p>
                                        <p>
                                            <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                                Reset Password
                                            </a>
                                        </p>
                                        <p>If you didn’t request this, you can safely ignore this email. Your password will remain unchanged.</p>
                                        <br />
                                        <p>Best regards,<br><strong>PennyPilot Team</strong></p>
                                    </body>
                                </html>";

            await _emailService.SendEmailAsync(user.Email, subject, emailBody);

            return new ServerResponse<string>
            {
                Data = "Email sent successfully",
                Success = true,
                Message = "Email sent successfully"
            }; ;
        }

        public async Task<ServerResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(request.Token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return new ServerResponse<string>
                {
                    Data = "Token Expired or Invalid",
                    Success = false,
                    Message = "Token Expired or Invalid"
                };

            // Hash new password - (replace with your password hasher)
            user.PasswordHash = _securityService.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServerResponse<string>
            {
                Data = "Password Changed Successfully",
                Success = true,
                Message = "Password Changed Successfully"
            };
        }

    }
}
