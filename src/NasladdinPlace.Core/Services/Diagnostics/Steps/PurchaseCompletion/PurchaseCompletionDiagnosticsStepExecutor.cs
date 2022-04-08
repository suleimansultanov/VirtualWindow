using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseCompletion
{
    public class PurchaseCompletionDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPurchaseManager _purchaseManager;

        public PurchaseCompletionDiagnosticsStepExecutor(IPurchaseManager purchaseManager)
        {
            if (purchaseManager == null)
                throw new ArgumentNullException(nameof(purchaseManager));

            _purchaseManager = purchaseManager;
        }
        
        public Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new NotSupportedException("Diagnostics context must have a user.");

            return ExecuteAuxAsync(context);
        }

        private async Task<Result> ExecuteAuxAsync(DiagnosticsContext context)
        {
            var userId = context.User.Id;

            var purchaseCompletionInitiationParams = new PurchaseOperationParams(userId);
            var purchaseCompletionResult = await _purchaseManager.InitiateCompletionAsync(purchaseCompletionInitiationParams);

            return purchaseCompletionResult;
        }
    }
}