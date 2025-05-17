using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class RegisterUserRequestDto
    {
        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;
        public DateTime DOB { get; set; } 

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
