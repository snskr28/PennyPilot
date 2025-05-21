using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IIncomeService
    {
        Task<Guid> AddIncomeAsync(Guid userId, AddIncomeDto dto);
        Task UpdateIncomeAsync(Guid userId, UpdateIncomeDto dto);
        Task DeleteIncomeAsync(Guid userId, Guid incomeId);
    }
}
