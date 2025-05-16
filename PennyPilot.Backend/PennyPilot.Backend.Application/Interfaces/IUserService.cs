using PennyPilot.Backend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IUserService
    {
        //Task<IEnumerable<UserDto>> GetAllUsersAsync();
        //Task<UserDto> GetUserByIdAsync(int id);
        //Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        //Task UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        //Task DeleteUserAsync(int id);
        //Task<IEnumerable<UserDto>> GetActiveUsersAsync();
        //Task<UserDto> GetUserByEmailAsync(string email);
        Task<ServerResponse<UserDto>> RegisterUserAsync(RegisterUserRequestDto requestDto);
    }
}
