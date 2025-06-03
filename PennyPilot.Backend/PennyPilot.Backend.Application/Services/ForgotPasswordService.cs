using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Services
{
    public class ForgotPasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ISecurityService _securityService;

        public ForgotPasswordService(IUnitOfWork unitOfWork, IEmailService emailService, ISecurityService securityService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _securityService = securityService;
        }

        public async Task<bool> SendPasswordResetTokenAsync(ForgotPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
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
