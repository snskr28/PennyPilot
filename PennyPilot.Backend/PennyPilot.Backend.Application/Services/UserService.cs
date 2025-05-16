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
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;
        public UserService(IUserRepository userRepository, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        //public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        //{
        //    var users = await _userRepository.GetAllAsync();
        //    return users.Select(user => MapToDto(user));
        //}

        //public async Task<UserDto> GetUserByIdAsync(int id)
        //{
        //    var user = await _userRepository.GetByIdAsync(id);
        //    return user == null ? null : MapToDto(user);
        //}

        //public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        //{
        //    var user = new User
        //    {
        //        FirstName = createUserDto.FirstName,
        //        LastName = createUserDto.LastName,
        //        Email = createUserDto.Email,
        //        DateOfBirth = createUserDto.DateOfBirth,
        //        IsActive = true,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    var createdUser = await _userRepository.AddAsync(user);
        //    return MapToDto(createdUser);
        //}

        //public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        //{
        //    var existingUser = await _userRepository.GetByIdAsync(id);

        //    if (existingUser == null)
        //        throw new KeyNotFoundException($"User with ID {id} not found");

        //    existingUser.FirstName = updateUserDto.FirstName;
        //    existingUser.LastName = updateUserDto.LastName;
        //    existingUser.Email = updateUserDto.Email;
        //    existingUser.DateOfBirth = updateUserDto.DateOfBirth;
        //    existingUser.IsActive = updateUserDto.IsActive;

        //    await _userRepository.UpdateAsync(existingUser);
        //}

        //public async Task DeleteUserAsync(int id)
        //{
        //    await _userRepository.DeleteAsync(id);
        //}

        //public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        //{
        //    var users = await _userRepository.GetActiveUsersAsync();
        //    return users.Select(user => MapToDto(user));
        //}

        //public async Task<UserDto> GetUserByEmailAsync(string email)
        //{
        //    var user = await _userRepository.GetUserByEmailAsync(email);
        //    return user == null ? null : MapToDto(user);
        //}

        //private static UserDto MapToDto(User user)
        //{
        //    return new UserDto
        //    {
        //        Id = user.Id,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        DateOfBirth = user.DateOfBirth,
        //        IsActive = user.IsActive,
        //        CreatedAt = user.CreatedAt
        //    };
        //}

        public async Task<ServerResponse<UserDto>> RegisterUserAsync(RegisterUserRequestDto requestDto)
        {
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(requestDto.Email);
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(requestDto.Username);
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
                Email = requestDto.Email,
                PasswordHash = _securityService.HashPassword(requestDto.Password), 
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false
            };

            var createdUser = await _userRepository.AddAsync(newUser);

            var userDto = new UserDto
            {
                Username = createdUser.Username,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt
            };

            return new ServerResponse<UserDto>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = userDto
            };
        }
    }
}
