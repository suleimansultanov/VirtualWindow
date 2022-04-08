using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation
{
    public class PurchaseInitiationDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPurchaseManager _purchaseManager;
        private readonly PurchaseInitiationDiagnosticsStepParams _purchaseInitiationDiagnosticsStepParams;

        public PurchaseInitiationDiagnosticsStepExecutor(
            IPurchaseManager purchaseManager,
            PurchaseInitiationDiagnosticsStepParams purchaseInitiationDiagnosticsStepParams)
        {
            if (purchaseManager == null)
                throw new ArgumentNullException(nameof(purchaseManager));
            if (purchaseInitiationDiagnosticsStepParams == null)
                throw new ArgumentNullException(nameof(purchaseInitiationDiagnosticsStepParams));

            _purchaseManager = purchaseManager;
            _purchaseInitiationDiagnosticsStepParams = purchaseInitiationDiagnosticsStepParams;
        }

        public Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseInitiationDiagnosticsStepExecutor)} must have a user."
                );

            return ExecuteAuxAsync(context);
        }

        private async Task<Result> ExecuteAuxAsync(DiagnosticsContext context)
        {
            var qrCode = _purchaseInitiationDiagnosticsStepParams.QrCode;

            var purchaseInitiationResult = await _purchaseManager.InitiateAsync(new PurchaseInitiationParams(
                context.User.Id,
                qrCode
            ));

            context.PosOperation = purchaseInitiationResult.PosOperation;

            return purchaseInitiationResult.Succeeded
                ? Utilities.Models.Result.Success()
                : Utilities.Models.Result.Failure(purchaseInitiationResult.Error);
        }
    }
}