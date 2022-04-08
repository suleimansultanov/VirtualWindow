using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.Checks.Contracts;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.UnpaidCheckDateTimeHelpers;
using NasladdinPlace.Logging;
using NasladdinPlace.Core.Models.PaymentCards;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize]
    public class ChecksController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICheckCorrectnessStatusProcessor _checkCorrectnessStatusProcessor;
        private readonly IPurchaseManager _purchaseManager;
        private readonly TimeSpan _additionalTimeSpan;
        private readonly ICheckService _checkService;
        private readonly ILogger _logger;
        private readonly IPaymentCardsService _paymentCardsService;

        public ChecksController(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _checkCorrectnessStatusProcessor = serviceProvider.GetRequiredService<ICheckCorrectnessStatusProcessor>();
            _purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();
            _additionalTimeSpan = serviceProvider.GetRequiredService<IConfigurationReader>()
                .GetNextPaymentAttemptTimeout();
            _checkService = serviceProvider.GetRequiredService<ICheckService>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _paymentCardsService = serviceProvider.GetRequiredService<IPaymentCardsService>();
        }

        [HttpGet("unpaid")]
        public async Task<IActionResult> GetUnpaidByUserAsync()
        {
            var userId = GetUserId();

            var checkResult = await _checkService.GetUnpaidByUserAsync(userId);

            return ToActionResult(checkResult);
        }

        [HttpGet("firstUnpaid")]
        public async Task<IActionResult> GetFirstUnpaidByUserAsync()
        {
            var userId = GetUserId();

            var checkResult = await _checkService.GetFirstUnpaidByUserAsync(userId);

            return ToActionResult(checkResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var userId = GetUserId();

            var checks = await _checkService.GetAllAsync(userId);

            return Ok(checks.Value);
        }

        [HttpPost("payCheck")]
        public async Task<IActionResult> PayCheckAsync([FromBody] UnpaidCheckInfo targetUnpaidCheckInfo)
        {
            var user = await _userManager.GetUserAsync(User);

            var isPaymentCardBelongsToUserResult =
                await _paymentCardsService.IsPaymentCardBelongsToTheUserAsync(user.Id, targetUnpaidCheckInfo.PaymentCardId);

            if (!isPaymentCardBelongsToUserResult.Succeeded)
                return BadRequest(isPaymentCardBelongsToUserResult.Error);

            var targetUnpaidCheckResult =
                await _checkService.GetUnpaidCheckResultByPosOperationAndUserIdAsync(user.Id,
                    targetUnpaidCheckInfo.PosOperationId);

            if (!targetUnpaidCheckResult.Succeeded)
                return BadRequest(targetUnpaidCheckResult.Error);

            var targetUnpaidCheck = targetUnpaidCheckResult.Value;

            var unpaidSimpleCheckForPayDtoResult = _checkService.MakeSimpleCheckDto(targetUnpaidCheck.Check);

            var unpaidSimpleCheckForPayDto = unpaidSimpleCheckForPayDtoResult.Value;

            var currentPaymentCardId = targetUnpaidCheckInfo.PaymentCardId ?? user.ActivePaymentCardId;

            var verifyPurchaseResult =
                _checkService.VerifyPurchaseIfCheckPayable(
                    unpaidSimpleCheckForPayDto, 
                    targetUnpaidCheck,
                    currentPaymentCardId, 
                    targetUnpaidCheck.Check.PaymentErrorInfo?.PaymentCardId);

            if (!verifyPurchaseResult.Succeeded)
            {
                var unpaidPurchaseCompletionResult = SetPaymentError(unpaidSimpleCheckForPayDto, verifyPurchaseResult.Error);

                return BadRequest(unpaidPurchaseCompletionResult);
            }

            if (targetUnpaidCheck.CheckPosOperation.Status != PosOperationStatus.Paid)
            {
                var purchaseCompletionResult =
                    await _purchaseManager.CompleteUnpaidPurchaseAsync(new PurchaseOperationParams(user.Id),
                        targetUnpaidCheckInfo.PosOperationId, currentPaymentCardId);

                if (!purchaseCompletionResult.IsSuccess)
                {
                    var bankTransactionError =
                        !string.IsNullOrEmpty(purchaseCompletionResult.Error.LocalizedDescription)
                            ? purchaseCompletionResult.Error.LocalizedDescription
                            : purchaseCompletionResult.Error.Description;
                    
                    var unpaidPurchaseCompletionResult = SetPaymentError(unpaidSimpleCheckForPayDto,
                        bankTransactionError, DateTime.UtcNow);

                    return Ok(unpaidPurchaseCompletionResult);
                }
            }

            var nextUnpaidCheckResult = await _checkService.GetNextUnpaidCheckByUserIdAsync(user.Id);

            return Ok(nextUnpaidCheckResult.Value);
        }

        [HttpPost("payment")]
        public async Task<IActionResult> PayForAllChecksAsync()
        {
            var userId = GetUserId();

            var purchaseCompletionResults = await _checkService.PayForAllChecksAsync(userId);

            return Ok(purchaseCompletionResults.Value);
        }

        [HttpPut("{id}/correctnessStatus")]
        public async Task<IActionResult> UpdateCorrectnessStatusAsync(int id, [FromBody] CorrectnessStatusUpdateEntityDto checkCorrectnessStatusUpdateEntityDto)
        {
            try
            {
                if (checkCorrectnessStatusUpdateEntityDto.Value != null &&
                    !Enum.IsDefined(typeof(CheckCorrectnessStatus), checkCorrectnessStatusUpdateEntityDto.Value.Value))
                    return BadRequest($"Incorrect value for {nameof(CheckCorrectnessStatus)}");

                var userId = GetUserId();

                var updateCorrectnessStatusResult = await _checkCorrectnessStatusProcessor.ProcessCorrectnessStatusForPosOperationAsync(
                    posOperationId: id,
                    correctnessStatus: (CheckCorrectnessStatus)checkCorrectnessStatusUpdateEntityDto.Value.Value,
                    userId: userId);

                if (!updateCorrectnessStatusResult.Succeeded)
                    return BadRequest(updateCorrectnessStatusResult.Error);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Some error has been occured during status updating. Verbose error = {ex}");
                return BadRequest("Some error has been occured during status updating.");
            }
        }

        private int GetUserId()
        {
            return _userManager.GetUserIdAsInt(User);
        }

        private UnpaidPurchaseCompletionResult SetPaymentError(SimpleCheckDto unpaidCheckResultDto,
            string error,
            DateTime? nextPaymentAttemptDate = null)
        {
            if ( unpaidCheckResultDto.PaymentError == null )
                unpaidCheckResultDto.PaymentError = new SimpleCheckPaymentErrorInfoDto();

            unpaidCheckResultDto.PaymentError.Message = error;

            if (nextPaymentAttemptDate.HasValue)
                unpaidCheckResultDto.PaymentError.NextPaymentAttemptDate =
                    DateTimeHelper.CalculateNextPaymentDateTime(nextPaymentAttemptDate.Value, _additionalTimeSpan);

            return UnpaidPurchaseCompletionResult.Failed(unpaidCheckResultDto.PaymentError);
        }

        private IActionResult ToActionResult(ValueResult<SimpleCheckDto> checkResult)
        {
            if (checkResult.Succeeded)
                return Ok(checkResult.Value);

            if (string.IsNullOrEmpty(checkResult.Error))
                return NotFound();

            return BadRequest(checkResult.Error);
        }
    }
}