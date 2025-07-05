using Microsoft.AspNetCore.Authorization;
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
    public class ChartsController : ControllerBase
    {
        private readonly IChartsService _chartsService;
        public ChartsController(IChartsService chartsService)
        {
            _chartsService = chartsService;
        }

        [HttpPost("DonutCharts")]
        public async Task<IActionResult> GetDonutChartsData(DashboardFilterDto dashboardFilter)
        {
            var response = new ServerResponse<DonutChartsDto>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _chartsService.GetDonutChartsData(userId, dashboardFilter);
                response.Success = true;
                response.Message = "Charts fetched successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("IncomeExpenseBarChart")]
        public async Task<IActionResult> GetIncomeExpenseBarChartsData(DashboardFilterDto dashboardFilter)
        {
            var response = new ServerResponse<BarChartResponseDto>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _chartsService.GetIncomeExpenseBarChartData(userId, dashboardFilter);
                response.Success = true;
                response.Message = "Bar Chart fetched successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("IncomeExpenseLineChart")]
        public async Task<IActionResult> GetIncomeExpenseLineChartsData(DashboardFilterDto dashboardFilter)
        {
            var response = new ServerResponse<LineChartResponseDto>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _chartsService.GetIncomeExpenseLineChartData(userId, dashboardFilter);
                response.Success = true;
                response.Message = "Line Chart fetched successfully.";
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
