using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class DonutChartsDto
    {
        public Dictionary<string, decimal>? ExpenseCategories { get; set; }        
        public Dictionary<string, decimal>? UserExpenses { get; set; }
        public Dictionary<string, decimal>? IncomeCategories { get; set; }
        public Dictionary<string, decimal>? IncomeSources { get; set; }
    }
}
