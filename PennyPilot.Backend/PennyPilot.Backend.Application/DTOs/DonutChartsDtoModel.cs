using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class DonutChartsDtoModel
    {
        public Dictionary<string, int>? ExpenseCategories { get; set; }
        public Dictionary<string, int>? IncomeCategories { get; set; }
    }
}
