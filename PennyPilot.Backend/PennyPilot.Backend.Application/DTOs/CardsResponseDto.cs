using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class CardsResponseDto
    {
        public CardDto? TotalIncome { get; set; }
        public CardDto? TotalExpenses { get; set; }
        public CardDto? NetSavings { get; set; }
    }

    public class CardDto
    {
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
    }
}
