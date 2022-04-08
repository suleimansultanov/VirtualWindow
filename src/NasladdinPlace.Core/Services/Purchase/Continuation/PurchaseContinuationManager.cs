using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.Continuation
{
    public class PurchaseContinuationManager : IPurchaseContinuationManager
    {
        private readonly IPosInteractor _posInteractor;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;

        public PurchaseContinuationManager(
            IPosInteractor posInteractor,
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager)
        {
            if (posInteractor == null)
                throw new ArgumentNullException(nameof(posInteractor));
            if (ongoingPurchaseActivityManager == null)
                throw new ArgumentNullException(nameof(ongoingPurchaseActivityManager));

            _posInteractor = posInteractor;
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
        }

        public Task<Result> ContinuePurchaseAsync(PurchaseOperationParams continuationParams)
        {
            if (continuationParams == null)
                throw new ArgumentNullException(nameof(continuationParams));

            return ContinuePurchaseAuxAsync(continuationParams);
        }

        private async Task<Result> ContinuePurchaseAuxAsync(PurchaseOperationParams continuationParams)
        {
            var userId = continuationParams.UserId;

            var result = await _posInteractor.ContinueOperationAsync(userId);

            if (result.Succeeded)
            {
                _ongoingPurchaseActivityManager.Users.StartTrackingActivity(userId);
                return Result.Success();
            }

            var failureCause = $"The user {userId} tried to open another door, but failed because {result.Error}.";

            return Result.Failure(failureCause);
        }
    }
}