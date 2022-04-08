using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Purchase.CompletionInitiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.CompletionInitiation
{
    public class PurchaseCompletionInitiationManager : IPurchaseCompletionInitiationManager
    {
        private readonly ITestUserInfoProvider _testUserInfoProvider;
        private readonly IPosInteractor _posInteractor;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;

        public PurchaseCompletionInitiationManager(
            ITestUserInfoProvider testUserInfoProvider,
            IPosInteractor posInteractor,
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager)
        {
            if (testUserInfoProvider == null)
                throw new ArgumentNullException(nameof(testUserInfoProvider));
            if (posInteractor == null)
                throw new ArgumentNullException(nameof(posInteractor));
            if (ongoingPurchaseActivityManager == null)
                throw new ArgumentNullException(nameof(ongoingPurchaseActivityManager));

            _testUserInfoProvider = testUserInfoProvider;
            _posInteractor = posInteractor;
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
        }

        public Task<Result> InitiatePurchaseCompletionAsync(PurchaseOperationParams initiationParams)
        {
            if (initiationParams == null)
                throw new ArgumentNullException(nameof(initiationParams));

            return InitiatePurchaseCompletionAuxAsync(initiationParams);
        }

        private async Task<Result> InitiatePurchaseCompletionAuxAsync(PurchaseOperationParams initiationParams)
        {
            var userId = initiationParams.UserId;

            if (await _testUserInfoProvider.IsTestUserAsync(userId))
                return Result.Success();

            var operationCompletionResult = await _posInteractor.TryCompleteOperationAndShowTimerOnDisplayAsync(userId);

            if (!operationCompletionResult.Succeeded)
                return Result.Failure($"Cannot close doors because: {operationCompletionResult.Error}");

            _ongoingPurchaseActivityManager.Users.StopTrackingActivity(userId);
            _ongoingPurchaseActivityManager.PointsOfSale.StopTrackingActivity(operationCompletionResult.PosOperation.PosId);

            return Result.Success();
        }
    }
}