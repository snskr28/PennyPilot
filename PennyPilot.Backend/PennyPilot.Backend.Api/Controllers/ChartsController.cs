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
    public class ChartsController : ControllerBase
    {
        private readonly IChartsService _chartsService;
        public ChartsController(IChartsService chartsService)
        {
            _chartsService = chartsService;
        }

        [HttpGet("DonutCharts")]
        public async Task<IActionResult> GetDonutChartsData()
        {
            var response = new ServerResponse<DonutChartsDtoModel>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                response.Data = await _chartsService.GetDonutChartsData(userId);
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
    }
}
