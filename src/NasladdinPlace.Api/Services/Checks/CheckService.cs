using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Api.Services.Checks.Contracts;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.UnpaidCheckDateTimeHelpers;

namespace NasladdinPlace.Api.Services.Checks
{
    public class CheckService : ICheckService
    {
        private readonly IPurchasesHistoryMaker _purchasesHistoryMaker;
        private readonly IUserLatestOperationCheckMaker _userLatestOperationCheckMaker;
        private readonly IPurchaseManager _purchaseManager;
        private readonly TimeSpan _additionalTimeSpan;

        public CheckService(IServiceProvider serviceProvider)
        {
            _purchasesHistoryMaker = serviceProvider.GetRequiredService<IPurchasesHistoryMaker>();
            _userLatestOperationCheckMaker = serviceProvider.GetRequiredService<IUserLatestOperationCheckMaker>();
            _purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();
            _additionalTimeSpan = serviceProvider.GetRequiredService<IConfigurationReader>()
                .GetNextPaymentAttemptTimeout();
        }

        public async Task<ValueResult<SimpleCheckDto>> GetUnpaidByUserAsync(int userId)
        {
            var checkResult = await _userLatestOperationCheckMaker.MakeForUserIfOperationUnpaidAsync(userId);

            return GetResult(checkResult);
        }

        public async Task<ValueResult<SimpleCheckDto>> GetFirstUnpaidByUserAsync(int userId)
        {
            var checkResult = await _userLatestOperationCheckMaker.MakeForUserIfFirstOperationUnpaidAsync(userId);

            return GetResult(checkResult);
        }

        public async Task<ValueResult<ImmutableList<SimpleCheckDto>>> GetAllAsync(int userId)
        {
            var purchasesHistory = await _purchasesHistoryMaker.MakeChecksWithItemsForUserAsync(userId);

            var checksDto = purchasesHistory.Checks.Select(ToDto).ToImmutableList();

            checksDto.ForEach(RecalculateNextPaymentDate);

            return ValueResult<ImmutableList<SimpleCheckDto>>.Success(checksDto);
        }

        public async Task<ValueResult<List<PurchaseCompletionResultDto>>> PayForAllChecksAsync(int userId)
        {
            var purchaseCompletionResults = await _purchaseManager.CompleteAllUnpaidAsync(new PurchaseOperationParams(userId));

            var purchaseCompletionResultDtos = Mapper.Map<List<PurchaseCompletionResultDto>>(purchaseCompletionResults);

            return ValueResult<List<PurchaseCompletionResultDto>>.Success(purchaseCompletionResultDtos);
        }

        public async Task<ValueResult<UnpaidPurchaseCompletionResult>> GetNextUnpaidCheckByUserIdAsync(int userId)
        {
            var nextUnpaidCheckResult =
                await _userLatestOperationCheckMaker.MakeForUserIfFirstOperationUnpaidAsync(userId);

            if (nextUnpaidCheckResult.Status == UserOperationCheckMakerStatus.PosOperationNotFound)
                return ValueResult<UnpaidPurchaseCompletionResult>.Success(UnpaidPurchaseCompletionResult.Succeeded());

            var nextUnpaidCheckResultDto = MakeSimpleCheckDto(nextUnpaidCheckResult.Check);

            return ValueResult<UnpaidPurchaseCompletionResult>.Success(
                UnpaidPurchaseCompletionResult.Succeeded(nextUnpaidCheckResultDto.Value));
        }

        public Result VerifyPurchaseIfCheckPayable(SimpleCheckDto unpaidCheckResultDto,
            UserLatestOperationCheckMakerResult unpaidCheckResult,
            int? currentPaymentCardId, int? lastPayAttemptPaymentCardId)
        {
            if (!currentPaymentCardId.HasValue || currentPaymentCardId == lastPayAttemptPaymentCardId)
            {
                if (unpaidCheckResultDto.PaymentError?.NextPaymentAttemptDate != null &&
                    unpaidCheckResultDto.PaymentError.NextPaymentAttemptDate > DateTime.UtcNow)
                    return Result.Failure("The time for payment has not yet arrived. Please try again later.");
            }

            if (unpaidCheckResult.CheckPosOperation.Status != PosOperationStatus.Completed &&
                unpaidCheckResult.CheckPosOperation.Status != PosOperationStatus.Paid)
            {
                if (unpaidCheckResult.CheckPosOperation.Status != PosOperationStatus.PendingPayment)
                    return Result.Failure(
                        "Internal server error. Unexpected pos operation status (not equal pending payments).");

                return Result.Failure("It’s not possible to pay a check, it may already have been paid.");
            }

            return Result.Success();
        }

        public async Task<ValueResult<UserLatestOperationCheckMakerResult>> GetUnpaidCheckResultByPosOperationAndUserIdAsync(int userId, int posOperationId)
        {
            var unpaidCheckResult = await _userLatestOperationCheckMaker.MakeForUserByUnpaidOperationAsync(userId, posOperationId);

            if (unpaidCheckResult.Status == UserOperationCheckMakerStatus.PosOperationNotFound)
                return ValueResult<UserLatestOperationCheckMakerResult>.Failure($"Pos operation with id = {posOperationId} not found.");

            return unpaidCheckResult.Status == UserOperationCheckMakerStatus.Success
                ? ValueResult<UserLatestOperationCheckMakerResult>.Success(unpaidCheckResult)
                : ValueResult<UserLatestOperationCheckMakerResult>.Failure($"{unpaidCheckResult.Status}");
        }
        
        public ValueResult<SimpleCheckDto> MakeSimpleCheckDto(SimpleCheck check)
        {
            var checkDto = ToDto(check);

            RecalculateNextPaymentDate(checkDto);

            return ValueResult<SimpleCheckDto>.Success(checkDto);
        }

        private ValueResult<SimpleCheckDto> GetResult(UserLatestOperationCheckMakerResult checkResult)
        {
            if (checkResult.Status == UserOperationCheckMakerStatus.PosOperationNotFound)
                return ValueResult<SimpleCheckDto>.Failure();

            if (checkResult.Status != UserOperationCheckMakerStatus.Success)
                return ValueResult<SimpleCheckDto>.Failure($"{checkResult.Status}");

            var checkDto = MakeSimpleCheckDto(checkResult.Check);

            return ValueResult<SimpleCheckDto>.Success(checkDto.Value);
        }

        private static SimpleCheckDto ToDto(SimpleCheck simpleCheck)
        {
            return Mapper.Map<SimpleCheckDto>(simpleCheck);
        }

        private void RecalculateNextPaymentDate(SimpleCheckDto checkDto)
        {
	        if ( checkDto?.PaymentError != null )
		        checkDto.PaymentError.NextPaymentAttemptDate =
			        DateTimeHelper.CalculateNextPaymentDateTime( checkDto.PaymentError.NextPaymentAttemptDate, _additionalTimeSpan );
        }
    }
}
