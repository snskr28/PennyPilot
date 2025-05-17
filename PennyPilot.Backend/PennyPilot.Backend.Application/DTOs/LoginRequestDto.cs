using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }

}
