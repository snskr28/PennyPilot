﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.Constants;

namespace PennyPilot.Backend.Application.DTOs
{
    public class DashboardFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Granularity { get; set; } = GranularityConstant.Yearly;
        public string? ExpenseCategory { get; set; }
        public string? IncomeCategory { get; set; }
        public string? UserExpense { get; set; }
        public string? IncomeSource { get; set; }
    }
}
