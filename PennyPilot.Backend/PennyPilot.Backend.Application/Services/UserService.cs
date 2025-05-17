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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityService _securityService;
        public UserService(IUnitOfWork unitOfWork, ISecurityService securityService)
        {
            _unitOfWork = unitOfWork;
            _securityService = securityService;
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

        public async Task<ServerResponse<UserDto>> GetCurrentUserProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServerResponse<UserDto>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var userProfile = new UserDto
            {                
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DOB = user.Dob,
                Email = user.Email
            };

            return new ServerResponse<UserDto>
            {
                Success = true,
                Data = userProfile,
                Message = "User profile fetched successfully."
            };
        }

    }
}
