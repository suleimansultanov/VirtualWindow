using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseContinuation
{
    public class PurchaseContinuationDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPurchaseManager _purchaseManager;

        public PurchaseContinuationDiagnosticsStepExecutor(IPurchaseManager purchaseManager)
        {
            if (purchaseManager == null)
                throw new ArgumentNullException(nameof(purchaseManager));

            _purchaseManager = purchaseManager;
        }
        
        public async Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PurchaseContinuationDiagnosticsStepExecutor)} must have a user.");

            var purchaseContinuationResult =
                await _purchaseManager.ContinuePurchaseAsync(new PurchaseOperationParams(context.User.Id));

            return purchaseContinuationResult.Succeeded
                ? Result.Success()
                : Result.Failure(purchaseContinuationResult.Error);
        }
    }
}