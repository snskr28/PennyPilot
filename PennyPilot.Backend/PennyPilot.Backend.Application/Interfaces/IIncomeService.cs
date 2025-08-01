﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IIncomeService
    {
        Task<ServerResponse<List<Guid>>> AddIncomesAsync(Guid userId, List<AddIncomeDto> dto);
        Task UpdateIncomeAsync(Guid userId, UpdateIncomeDto dto);
        Task DeleteIncomeAsync(Guid userId, Guid incomeId);
        Task<TableResponseDto<IncomeTableDto>> GetUserIncomesAsync(Guid userId, TableRequestDto request);
    }
}
