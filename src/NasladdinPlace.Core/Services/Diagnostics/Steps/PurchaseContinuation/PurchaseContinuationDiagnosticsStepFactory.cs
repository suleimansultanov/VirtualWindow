using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseContinuation
{
    public class PurchaseContinuationDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        public PurchaseContinuationDiagnosticsStepFactory(IServiceScope serviceScope) 
            : base(serviceScope)
        {
        }
        
        public override DiagnosticsStep Create()
        {
            var purchaseDiagnosticsStepInfo = new PurchaseContinuationDiagnosticsStepInfo();

            var purchaseContinuationDiagnosticsStepPreparer = new NoDiagnosticsStepPreparation();
            var purchaseContinuationDiagnosticsStepExecutor = new PurchaseContinuationDiagnosticsStepExecutor(
                GetRequiredService<IPurchaseManager>()
            );
            var purchaseContinuationDiagnosticsStepChecker = new PurchaseContinuationDiagnosticsStepChecker(
                GetRequiredService<IUnitOfWorkFactory>(),
                GetRequiredService<IOngoingPurchaseActivityManager>()
            );
            var purchaseContinuationDiagnosticsStepCleaner = new PurchaseContinuationDiagnosticsStepCleaner(
                GetRequiredService<IPosRemoteControllerFactory>()
            );

            return new DiagnosticsStep(
                purchaseDiagnosticsStepInfo,
                purchaseContinuationDiagnosticsStepPreparer,
                purchaseContinuationDiagnosticsStepExecutor,
                purchaseContinuationDiagnosticsStepChecker,
                purchaseContinuationDiagnosticsStepCleaner
            );
        }
    }
}