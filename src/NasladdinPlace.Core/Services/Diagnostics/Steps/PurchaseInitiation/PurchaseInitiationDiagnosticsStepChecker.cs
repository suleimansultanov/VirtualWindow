using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Diagnostics.Constants;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation
{
    public class PurchaseInitiationDiagnosticsStepChecker : IDiagnosticsStepSuccessChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;

        public PurchaseInitiationDiagnosticsStepChecker(
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

        public async Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            if (context.PosOperation == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseInitiationDiagnosticsStepChecker)} must have a pos operation.");
            
            var userId = context.PosOperation.UserId;
            var posId = context.PosOperation.PosId;

            _ongoingPurchaseActivityManager.Users.StopTrackingActivity(userId);
            _ongoingPurchaseActivityManager.PointsOfSale.StopTrackingActivity(posId);
            
            Task.Delay(DiagnosticsConstants.OpeningDoorTimeout).Wait();
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {   
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);

                if (posRealTimeInfo.DoorsState != DoorsState.LeftDoorOpened)
                    return Result.Failure(
                        $"After purchase initiation POS's {posId} left door must be opened."
                    );

                var posOperations =
                    await unitOfWork.PosOperations.GetByUserIncludingCheckItemsOrderedByDateStartedAsync(userId);

                if (!posOperations.Any())
                    return Result.Failure(
                        "After purchase initiation " +
                        $"an operation of the user {userId} and POS {posId} must be created."
                    );

                if (posOperations.Count != 1)
                    return Result.Failure(
                        "After purchase initiation " +
                        $"single operation of the user {userId} and POS {posId} must be created."
                    );

                var posOperation = posOperations.First();
                if (posOperation.DateCompleted != null)
                    return Result.Failure(
                        "After purchase initiation " +
                        $"completed operations of the user {userId} and POS {posId} must not exist."
                    );

                return Result.Success();
            }
        }
    }
}