using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.ProteinsFatsCarbohydratesCalories;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.MicroNutrients.Contracts;
using NasladdinPlace.Api.Services.MicroNutrients.Models;
using NasladdinPlace.Core.Models;
using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize]
    public class NutrientsController : Controller
    {
        private readonly INutrientsService _nutrientsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NutrientsController(
            INutrientsService nutrientsService,
            UserManager<ApplicationUser> userManager)
        {
            if (nutrientsService == null)
                throw new ArgumentNullException(nameof(nutrientsService));
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            _nutrientsService = nutrientsService;
            _userManager = userManager;
        }

        [HttpPost("history")]
        public async Task<IActionResult> GetProductsHistory([FromBody] NutrientsHistoryDto historyDto)
        {
            if (historyDto.CountOfDays <= 0)
                return BadRequest($"{nameof(historyDto.CountOfDays)} must be greater than 0.");

            if (historyDto.WorkDate.Kind != DateTimeKind.Utc)
                return BadRequest($"{nameof(historyDto.WorkDate)} is not in UTC format. Please check the entered date.");

            var userId = _userManager.GetUserIdAsInt(User);

            var productsHistoryResult =
                await _nutrientsService.GetNutrientsHistoryByUserAsync(userId, historyDto.WorkDate, historyDto.CountOfDays);

            if (productsHistoryResult.Succeeded)
                return Ok(productsHistoryResult.Value);

            return BadRequest(productsHistoryResult.Error);
        }

        [HttpPost("goals")]
        public async Task<IActionResult> SetUserGoals([FromBody] UserParams userParams)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            userParams.SetUserId(userId);

            var nutrientsResult = await _nutrientsService.GetNutrientsByUserParametersAsync(userParams);

            if (nutrientsResult.Succeeded)
                return Ok(nutrientsResult.Value);

            return BadRequest(nutrientsResult.Error);
        }

        [HttpGet("goals")]
        public async Task<IActionResult> GetUserGoals()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var nutrientsAndGoalsResult = await _nutrientsService.GetUserGoalsAsync(userId);

            if (nutrientsAndGoalsResult.Succeeded)
                return Ok(nutrientsAndGoalsResult.Value);

            return BadRequest(nutrientsAndGoalsResult.Error);
        }
    }
}