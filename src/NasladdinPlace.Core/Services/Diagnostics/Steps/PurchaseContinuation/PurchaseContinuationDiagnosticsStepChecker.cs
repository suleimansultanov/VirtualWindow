using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Diagnostics.Constants;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseContinuation
{
    public class PurchaseContinuationDiagnosticsStepChecker : IDiagnosticsStepSuccessChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;

        public PurchaseContinuationDiagnosticsStepChecker(
            IUnitOfWorkFactory unitOfWorkFactory,
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (ongoingPurchaseActivityManager == null)
                throw new ArgumentNullException(nameof(ongoingPurchaseActivityManager));

            _unitOfWorkFactory = unitOfWorkFactory;
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
        }

        public Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            if (context.PosOperation == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseContinuationDiagnosticsStepChecker)} must have a pos operation.");

            var posOperation = context.PosOperation;
            var posId = posOperation.PosId;
            var userId = posOperation.UserId;
            
            _ongoingPurchaseActivityManager.Users.StopTrackingActivity(userId);
            _ongoingPurchaseActivityManager.PointsOfSale.StopTrackingActivity(posId);

            WaitForOpeningDoorTimeout();
            
            return Task.FromResult(CheckRightDoorOpened(posId));
        }

        private void WaitForOpeningDoorTimeout()
        {
            Task.Delay(DiagnosticsConstants.OpeningDoorTimeout).Wait();
        }

        private Result CheckRightDoorOpened(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
                if (posRealTimeInfo.DoorsState != DoorsState.RightDoorOpened)
                {
                    return Result.Failure($"After purchase continuation the right door of POS {posId} must be opened.");
                }
            }
            
            return Result.Success();
        }
    }
}