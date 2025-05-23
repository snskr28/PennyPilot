using PennyPilot.Backend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<Guid> AddExpenseAsync(Guid userId, AddExpenseDto dto);
        Task UpdateExpenseAsync(Guid userId, UpdateExpenseDto dto);
        Task DeleteExpenseAsync(Guid userId, Guid expenseId);       
        Task<TableResponseDto<ExpenseTableDto>> GetUserExpensesAsync(Guid userId, TableRequestDto request);
    
    }
}
