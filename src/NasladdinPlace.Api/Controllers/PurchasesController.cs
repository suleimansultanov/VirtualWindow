using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Dtos.PurchasesFinisher;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.Brand;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.Core.Services.PurchasesResetter;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Dtos.Purchase;
using Newtonsoft.Json;
using Serilog;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPurchasesHistoryMaker _purchasesHistoryMaker;
        private readonly IUserPurchasesResetter _unfinishedUserPurchaseResetter;
        private readonly ILogger _logger;
        private readonly IPurchaseManager _purchaseManager;
        private readonly IUnpaidPurchaseFinisher _unpaidPurchaseFinisher;

        public PurchasesController(
            UserManager<ApplicationUser> userManager,
            IPurchasesHistoryMaker purchasesHistoryMaker,
            IUserPurchasesResetter unfinishedUserPurchaseResetter,
            ILogger logger,
            IPurchaseManager purchaseManager,
            IUnpaidPurchaseFinisher unpaidPurchaseFinisher)
        {
            _userManager = userManager;
            _purchasesHistoryMaker = purchasesHistoryMaker;
            _unfinishedUserPurchaseResetter = unfinishedUserPurchaseResetter;
            _logger = logger;
            _purchaseManager = purchaseManager;
            _unpaidPurchaseFinisher = unpaidPurchaseFinisher;
        }

        [HttpPost("api/purchases")]
        public async Task<IActionResult> InitiatePurchaseAsync([FromBody] PurchaseInitiationRequestDto dto)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            PosDoorPosition? doorPosition= null;

            if (dto.DoorPosition.HasValue)
            {
                if (Enum.IsDefined(typeof(PosDoorPosition), dto.DoorPosition))
                    doorPosition = (PosDoorPosition?)dto.DoorPosition.Value;
                else
                    return BadRequest(
                        $"Unable to find the specified {nameof(PosDoorPosition)} {dto.DoorPosition}.");
            }

            var purchaseInitiationResult =
                await _purchaseManager.InitiateAsync(new PurchaseInitiationParams(userId, dto.QrCode, doorPosition)
                {
                    Brand = HttpContext.Request.GetBrandHeaderValue()
                });

            if (!purchaseInitiationResult.Succeeded)
            {
                _logger.Error($"An error occurred during initialization of purchase of user {userId}:" +
                              $" {purchaseInitiationResult.Error}. " +
                              $"Initiation status is {purchaseInitiationResult.Status.ToString()}."
                );
            }
            else
            {
                var posId = purchaseInitiationResult.PosOperation.PosId;

                _logger.Information(
                    $"The user {userId} has been successfully initiated operation with POS {posId} by " +
                    $"providing QR code {dto.QrCode}.");
            }

            var purchaseInitiationResultDto = Mapper.Map<PurchaseInitiationResultDto>(purchaseInitiationResult);

            return Ok(purchaseInitiationResultDto);
        }

        [HttpPost("api/[controller]/mine")]
        public async Task<IActionResult> CompleteUserPurchaseAsync()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            _logger.Information(
                $"The user {userId} entered in {nameof(CompleteUserPurchaseAsync)} method");

            var purchaseCompletionResult = await _purchaseManager.CompleteLastUnpaidAsync(new PurchaseOperationParams(userId));

            var purchaseCompletionResultDto = Mapper.Map<PurchaseCompletionResultDto>(purchaseCompletionResult);

            var purchaseCompletionResultJson = JsonConvert.SerializeObject(purchaseCompletionResultDto);

            _logger.Information(
                $"The user {userId} has been completed purchase having " +
                $"PosOperationId = {purchaseCompletionResult?.Operation?.Id} " +
                $"with the following result: {purchaseCompletionResultJson}");

            return Ok(purchaseCompletionResultDto);
        }

        [HttpGet("api/[controller]/mine")]
        public async Task<IActionResult> GetUserPurchasesHistoryAsync()
        {
            var userId = _userManager.GetUserIdAsInt(User);
            var purchasesHistory = await _purchasesHistoryMaker.MakeNonEmptyChecksForUserAsync(userId);
            var purchasesHistoryDto = Mapper.Map<PurchasesHistoryDto>(purchasesHistory);
            return Ok(purchasesHistoryDto);
        }

        [HttpDelete("api/users/{userId}/[controller]/unfinished")]
        [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
        public async Task<IActionResult> ResetUnfinishedUserPurchase(int userId)
        {
            await _unfinishedUserPurchaseResetter.Reset(userId);

            _logger.Information($"The current balance of a user {userId} has been successfully resetted.");

            return Ok();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("api/[controller]/allunpaidfinisher")]
        public async Task<IActionResult> FinishUnpaidPurchasesAsync([FromBody] PurchasesFinisherDto purchasesFinisherDto)
        {
            await _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(purchasesFinisherDto.ConsiderUnpaidAfter);
            return Ok();
        }
    }
}