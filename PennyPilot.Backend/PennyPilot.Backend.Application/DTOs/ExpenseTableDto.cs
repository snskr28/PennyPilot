using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class ExpenseTableDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public string PaidBy { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
