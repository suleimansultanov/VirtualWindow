using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Pos.RemoteController;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseContinuation
{
    public class PurchaseContinuationDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        private readonly IPosRemoteControllerFactory _posRemoteControllerFactory;

        public PurchaseContinuationDiagnosticsStepCleaner(IPosRemoteControllerFactory posRemoteControllerFactory)
        {
            _posRemoteControllerFactory = posRemoteControllerFactory ??
                                          throw new ArgumentNullException(nameof(posRemoteControllerFactory));
        }

        public async Task CleanUpAsync(DiagnosticsContext context)
        {
            if (context.PosOperation == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseContinuationDiagnosticsStepCleaner)} must have a pos operation.");

            var posRemoteController = _posRemoteControllerFactory.Create(context.PosOperation.PosId);
            await posRemoteController.CompleteOperationAsync();
        }
    }
}