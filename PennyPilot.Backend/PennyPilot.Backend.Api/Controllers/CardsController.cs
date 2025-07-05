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
    public class CardsController : ControllerBase
    {
        private readonly ICardsService _cardsService;
        public CardsController(ICardsService cardsService)
        {
            _cardsService = cardsService;
        }

        [HttpPost]
        public async Task<IActionResult> GetSummaryCardsData(DashboardFilterDto dashboardFilter)
        {
            var response = new ServerResponse<SummaryCardsResponseDto>();
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }
                throw new Exception();
                response.Data = await _cardsService.GetSummaryCards(userId, dashboardFilter);
                response.Success = true;
                response.Message = "Cards fetched successfully.";
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
