﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Api.Helpers;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Services;

namespace PennyPilot.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddIncomes([FromBody] List<AddIncomeDto> dto)
        {
            var response = new ServerResponse<List<Guid>>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response = await _incomeService.AddIncomesAsync(userId, dto);          
                response.Message = "Incomes added successfully.";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Success = false;
                return StatusCode(500, response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIncome([FromBody] UpdateIncomeDto dto)
        {
            var response = new ServerResponse<string>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                await _incomeService.UpdateIncomeAsync(userId, dto);
                response.Success = true;
                response.Message = "Income updated successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{incomeId:guid}")]
        public async Task<IActionResult> DeleteIncome(Guid incomeId)
        {
            var response = new ServerResponse<string>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                await _incomeService.DeleteIncomeAsync(userId, incomeId);
                response.Success = true;
                response.Message = "Income deleted successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("IncomeTable")]
        public async Task<IActionResult> GetIncomeTable([FromBody] TableRequestDto requestDto)
        {
            var response = new ServerResponse<TableResponseDto<IncomeTableDto>>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _incomeService.GetUserIncomesAsync(userId, requestDto);
                response.Success = true;
                response.Message = "Records fetched successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
