using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PennyPilot.Backend.Api.Helpers;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;

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
        public async Task<IActionResult> AddIncome([FromBody] AddIncomeDto dto)
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

                var incomeId = await _incomeService.AddIncomeAsync(userId, dto);
                response.Data = incomeId;
                response.Message = "Income added successfully.";
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
    }
}
