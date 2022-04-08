using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Audit;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.ViewModels.Checks;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    [Permission(nameof(DocumentGoodsMovingPermission))]
    public class ChecksController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICheckManager _checkManager;
        private readonly ILogger _logger;

        public ChecksController(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
            : base(serviceProvider)
        {
            _userManager = userManager;
            _checkManager = serviceProvider.GetRequiredService<ICheckManager>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [HttpPost("{checkId}/deletion")]
        public async Task<IActionResult> PerformCheckItemsDeletion(int checkId,
            [FromBody] CheckItemIdsFormViewModel checkItemIdsFormViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();

            _logger.Information(
                $"The user {user.Id} entered in PerformCheckItemsDeletion method. CheckId = {checkId}.");

            var checkItemDeletionInfo =
                 CheckItemsEditingInfo.ForAdmin(checkId, user.Id, checkItemIdsFormViewModel.CheckItemsIds);

            var refundOrDeleteCheckManagerResult =
                await _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionInfo);

            if (!refundOrDeleteCheckManagerResult.IsSuccessful)
            {
                _logger.Error(
                    $"Error has been occurred while during PerformCheckItemsDeletion method. UserId = {user.Id} checkId = {checkId}." +
                    $"ErrorMessage = {refundOrDeleteCheckManagerResult.Error}");
                return BadRequest(refundOrDeleteCheckManagerResult.Error);
            }

            _logger.Information(
                $"The user {user.Id} has been successfully finished PerformCheckItemsDeletion method. CheckId = {checkId}.");

            return Ok();
        }

        [HttpPost("{checkId}/confirmation")]
        public async Task<IActionResult> PerformCheckItemsConfirmation(int checkId,
            [FromBody] CheckItemIdsFormViewModel checkItemIdsFormViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();

            _logger.Information(
                $"The user {user.Id} entered in PerformCheckItemsConfirmation method. CheckId = {checkId}.");

            var checkItemConfirmationInfo =
                CheckItemsEditingInfo.ForAdmin(checkId, user.Id, checkItemIdsFormViewModel.CheckItemsIds);

            var confirmationResult =
                await _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemConfirmationInfo);

            if (!confirmationResult.IsSuccessful)
            {
                _logger.Error(
                    $"Error has been occurred while during PerformCheckItemsConfirmation method. UserId = {user.Id} checkId = {checkId}." +
                    $"ErrorMessage = {confirmationResult.Error}");
                return BadRequest(confirmationResult.Error);
            }

            _logger.Information(
                $"The user {user.Id} has been successfully finished PerformCheckItemsConfirmation method. CheckId = {checkId}.");

            return Ok();
        }

        [HttpPost("{checkId}/addition")]
        public async Task<IActionResult> PerformCheckItemAddition(int checkId,
            [FromBody] CheckItemAdditionFormViewModel checkItemAdditionFormViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();

            _logger.Information(
                $"The user {user.Id} entered in PerformCheckItemAddition method. CheckId = {checkId}.");

            var labeledGoodId = checkItemAdditionFormViewModel.LabelGoodId.Value;

            var labeledGood = await UnitOfWork.LabeledGoods.GetIncludingGoodAndCurrencyByIdAsync(labeledGoodId);

            if (labeledGood == null ||
                !labeledGood.CurrencyId.HasValue ||
                !labeledGood.GoodId.HasValue ||
                !labeledGood.Price.HasValue ||
                !labeledGood.PosId.HasValue)
            {
                var errorMessage = $"The label {labeledGoodId} does not contains data to add to the check.";
                _logger.Error(
                    $"Error has been occurred while during PerformCheckItemAddition method. UserId = {user.Id} checkId = {checkId}." +
                    $"ErrorMessage = {errorMessage}");

                return BadRequest(errorMessage);
            }

            labeledGood.MarkAsUsedInPosOperation(checkId);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                checkId,
                labeledGood.GoodId.Value,
                labeledGoodId,
                labeledGood.CurrencyId.Value,
                user.Id,
                labeledGood.Price.Value
            );

            var additionCheckManagerResult = await _checkManager.AddItemAsync(checkItemAdditionInfo);

            if (!additionCheckManagerResult.IsSuccessful)
            {
                _logger.Error(
                    $"Error has been occurred while during PerformCheckItemAddition method. UserId = {user.Id} checkId = {checkId}." +
                    $"ErrorMessage = {additionCheckManagerResult.Error}");
                return BadRequest(additionCheckManagerResult.Error);
            }

            await UnitOfWork.CompleteAsync();

            _logger.Information(
                $"The user {user.Id} has been successfully finished PerformCheckItemAddition method. CheckId = {checkId}.");

            return Ok();
        }

        [HttpGet("{checkId}/auditHistory")]
        public async Task<IActionResult> GetAuditHistoryAsync(int checkId)
        {
            var auditRecords = await UnitOfWork.CheckItemAuditRecords
                .GetIncludingCheckItemsAndUserByPosOperationIdOrderedByCreatedDateAsync(checkId);

            var result = auditRecords.Select(Mapper.Map<CheckItemAuditRecordDto>);

            return Ok(result);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}