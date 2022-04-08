using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Check.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using Newtonsoft.Json;
using Serilog;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/purchases/current")]
    public class CurrentPurchaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;
        private readonly ICheckCorrectnessStatusProcessor _checkCorrectnessStatusProcessor;
        private readonly IUserLatestOperationCheckMaker _userLatestOperationCheckMaker;

        public CurrentPurchaseController(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _ongoingPurchaseActivityManager = serviceProvider.GetRequiredService<IOngoingPurchaseActivityManager>();
            _checkCorrectnessStatusProcessor = serviceProvider.GetRequiredService<ICheckCorrectnessStatusProcessor>();
            _userLatestOperationCheckMaker = serviceProvider.GetRequiredService<IUserLatestOperationCheckMaker>();
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentPurchaseCheckAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var userId = user.Id;

            var checkResult = await _userLatestOperationCheckMaker.MakeForUserIfOperationUnpaidAsync(userId);

            var checkDto = Mapper.Map<CheckDto>(checkResult.Check ?? SimpleCheck.Empty);

            _logger.Information($"The check is send to user {userId}. " +
                                $"Check: {JsonConvert.SerializeObject(checkDto)}");

            return Ok(checkDto);
        }

        [HttpPut("activity")]
        public IActionResult UpdateUserActivity()
        {
            var userId = GetUserId();

            _ongoingPurchaseActivityManager.Users.UpdateActivity(userId);

            return Ok();
        }
        
        [HttpPut]
        public async Task<IActionResult> RecheckCurrentPurchaseAsync()
        {
            var userId = GetUserId();

            var recheckLastPurchaseResult = await _checkCorrectnessStatusProcessor.RecheckLastPurchaseForUserAsync(userId);

            if (!recheckLastPurchaseResult.Succeeded)
                return BadRequest(recheckLastPurchaseResult.Error);

            return Ok();
        }

        private int GetUserId()
        {
            return _userManager.GetUserIdAsInt(User);
        }
    }
}