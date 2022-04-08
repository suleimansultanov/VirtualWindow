using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Pos.RemoteController;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation
{
    public class PurchaseInitiationDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosRemoteControllerFactory _posRemoteControllerFactory;

        public PurchaseInitiationDiagnosticsStepCleaner(
            IOngoingPurchaseActivityManager ongoingPurchaseActivityManager,
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosRemoteControllerFactory posRemoteControllerFactory)
        {
            if (ongoingPurchaseActivityManager == null)
                throw new ArgumentNullException(nameof(ongoingPurchaseActivityManager));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posRemoteControllerFactory == null)
                throw new ArgumentNullException(nameof(posRemoteControllerFactory));

            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
            _unitOfWorkFactory = unitOfWorkFactory;
            _posRemoteControllerFactory = posRemoteControllerFactory;
        }
        
        public Task CleanUpAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseInitiationDiagnosticsStepCleaner)} must have a user."
                );

            return CleanUpAuxAsync(context);
        }

        private async Task CleanUpAuxAsync(DiagnosticsContext context)
        {
            var userId = context.User.Id;

            _ongoingPurchaseActivityManager.Users.StopTrackingActivity(userId);

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperations =
                    await unitOfWork.PosOperations.GetByUserIncludingCheckItemsOrderedByDateStartedAsync(userId);

                await DeletePosDoorsStatesAsync(unitOfWork, posOperations);

                foreach (var posOperation in posOperations)
                {
                    unitOfWork.PosOperations.Remove(posOperation);
                }

                await unitOfWork.CompleteAsync();

                var pos = context.PosOperation?.Pos;
                if (pos != null)
                {
                    var posRemoteController = _posRemoteControllerFactory.Create(pos.Id);
                    await posRemoteController.CompleteOperationAsync();
                }
            }
        }

        private async Task DeletePosDoorsStatesAsync(IUnitOfWork unitOfWork, IEnumerable<PosOperation> posOperations)
        {
            var doorsStates = posOperations.SelectMany(po => po.PosDoorsStates);
            unitOfWork.PosDoorsStates.RemoveRange(doorsStates);
            await unitOfWork.CompleteAsync();
        }
    }
}