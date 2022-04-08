using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseCompletion
{
    public class PurchaseCompletionDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        public PurchaseCompletionDiagnosticsStepFactory(IServiceScope serviceScope) : base(serviceScope)
        {
        }
        
        public override DiagnosticsStep Create()
        {
            var purchaseCompletionDiagnosticsStepInfo = new PurchaseCompletionDiagnosticsStepInfo();
            var purchaseCompletionDiagnosticsStepPreparer = new NoDiagnosticsStepPreparation();
            var purchaseCompletionDiagnosticsStepExecutor = new PurchaseCompletionDiagnosticsStepExecutor(
                GetRequiredService<IPurchaseManager>()
            );
            var purchaseCompletionDiagnosticsStepChecker = new PurchaseCompletionDiagnosticsStepChecker(
                GetRequiredService<IUnitOfWorkFactory>()
            );
            var purchaseCompletionDiagnosticsStepCleaner = new PurchaseCompletionDiagnosticsStepCleaner(
                GetRequiredService<IUnitOfWorkFactory>()
            );

            return new DiagnosticsStep(
                purchaseCompletionDiagnosticsStepInfo,
                purchaseCompletionDiagnosticsStepPreparer,
                purchaseCompletionDiagnosticsStepExecutor,
                purchaseCompletionDiagnosticsStepChecker,
                purchaseCompletionDiagnosticsStepCleaner
            );
        }
    }
}