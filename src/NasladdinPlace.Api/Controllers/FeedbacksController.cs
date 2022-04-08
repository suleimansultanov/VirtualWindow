using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Application.Dtos.Feedback;
using NasladdinPlace.Application.Services.Feedbacks.Contracts;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    public class FeedbacksController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFeedbackAppService _feedbackAppService;

        public FeedbacksController(
            UserManager<ApplicationUser> userManager,
            IFeedbackAppService feedbackAppService)
        {
            _userManager = userManager;
            _feedbackAppService = feedbackAppService;
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedbackAsync([FromBody] FeedbackDto feedbackDto)
        {
            var user = await _userManager.GetUserAsync(User);
            await _feedbackAppService.CreateFeedbackAsync(feedbackDto, user);

            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost("unauthorized")]
        public async Task<IActionResult> AddFeedbackUnauthorizedAsync([FromBody] FeedbackDto feedbackDto)
        {
            await _feedbackAppService.CreateFeedbackAsync(feedbackDto);

            return Ok();
        }
    }
}