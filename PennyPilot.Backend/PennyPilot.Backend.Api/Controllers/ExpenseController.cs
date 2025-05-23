using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Api.Helpers;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using System.Security.Claims;

namespace PennyPilot.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] AddExpenseDto dto)
        {
            var response = new ServerResponse<Guid>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                var expenseId = await _expenseService.AddExpenseAsync(userId, dto);
                response.Data = expenseId;
                response.Message = "Expense added successfully.";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = Guid.Empty;
                response.Message = ex.Message;
                response.Success = false;
                return StatusCode(500, response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpense([FromBody] UpdateExpenseDto dto)
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

                await _expenseService.UpdateExpenseAsync(userId, dto);
                response.Success = true;
                response.Message = "Expense updated successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{expenseId:guid}")]
        public async Task<IActionResult> DeleteExpense(Guid expenseId)
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

                await _expenseService.DeleteExpenseAsync(userId, expenseId);
                response.Success = true;
                response.Message = "Expense deleted successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("ExpenseTable")]
        public async Task<IActionResult> GetExpensesTable([FromBody] TableRequestDto requestDto)
        {
            var response = new ServerResponse<TableResponseDto<ExpenseTableDto>>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _expenseService.GetUserExpensesAsync(userId, requestDto);
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
