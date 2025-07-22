using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class UpdateIncomeDto : AddIncomeDto
    {
        public Guid Incomeid { get; set; }
    }
}
