using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.PosDiagnostics.Contracts;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PosDiagnostics
{
    public class PurchasePosDiagnostics : IPosDiagnostics
    {
        private const int DoorOperationTimeoutInSeconds = 15;
        private const int InventoryTimeoutInSeconds = 30;
        
        private readonly IPosInteractor _posInteractor;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly PosDiagnosticsContext _context;

        public PurchasePosDiagnostics(
            IPosInteractor posInteractor,
            IUnitOfWorkFactory unitOfWorkFactory,
            PosDiagnosticsContext context)
        {
            _posInteractor = posInteractor;
            _unitOfWorkFactory = unitOfWorkFactory;
            _context = context;
        }

        public async Task<Result> PerformAsync()
        {
            var openingLeftDoorOperationResult = await StartPurchaseAsync();

            if (!openingLeftDoorOperationResult.Succeeded)
                return Result.Failure($"Cannot open left door because: {openingLeftDoorOperationResult.Error}");

            Wait(DoorOperationTimeoutInSeconds);

            var doorClosingResult = await CloseDoorsAsync();
            if (!doorClosingResult.Succeeded)
                return Result.Failure($"Cannot close left door because: {doorClosingResult.Error}");
            
            Wait(DoorOperationTimeoutInSeconds + InventoryTimeoutInSeconds);

            if (!await IsRecentCompletedUserAndPosOperationExistsAsync())
                return Result.Failure("Recent pos operation does not exist.");
            
            return Result.Success();
        }

        private Task<PosInteractionResult> StartPurchaseAsync()
        {
            var posOperationInitiationParams =
                PosOperationInitiationParams.ForPurchase(_context.UserId, _context.PosId, Brand.Nasladdin);
            return _posInteractor.InitiatePosOperationAsync(posOperationInitiationParams);
        }

        private Task<PosInteractionResult> CloseDoorsAsync()
        {
            return _posInteractor.TryCompleteOperationAndShowTimerOnDisplayAsync(_context.UserId);
        }

        private async Task<bool> IsRecentCompletedUserAndPosOperationExistsAsync()
        {
            PosOperation posOperation;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                posOperation = await unitOfWork.PosOperations.GetLatestActiveOfPosAsync(_context.PosId);
            }

            return posOperation != null &&
                   posOperation.UserId == _context.UserId &&
                   posOperation.PosId == _context.PosId &&
                   posOperation.DateCompleted.HasValue &&
                   DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(InventoryTimeoutInSeconds)) <
                   posOperation.DateCompleted;
        }

        private static void Wait(int seconds)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
        }
    }
}