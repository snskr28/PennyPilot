using PennyPilot.Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class AddExpenseDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = null!;
        public string? PaidBy { get; set; }
        public DateTime Date { get; set; }
        public byte[]? ReceiptImage { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
