using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation
{
    public class PurchaseInitiationDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        private readonly PurchaseInitiationDiagnosticsStepParams _purchaseInitiationDiagnosticsStepParams;

        public PurchaseInitiationDiagnosticsStepFactory(
            IServiceScope serviceScope,
            PurchaseInitiationDiagnosticsStepParams purchaseInitiationDiagnosticsStepParams)
            : base(serviceScope)
        {
            if (purchaseInitiationDiagnosticsStepParams == null)
                throw new ArgumentNullException(nameof(purchaseInitiationDiagnosticsStepParams));

            _purchaseInitiationDiagnosticsStepParams = purchaseInitiationDiagnosticsStepParams;
        }

        public override DiagnosticsStep Create()
        {
            var diagnosticsStepInfo = new PurchaseInitiationDiagnosticsStepInfo();

            var diagnosticsStepPreparer = new NoDiagnosticsStepPreparation();
            var purchaseInitiationDiagnosticsStepExecutor = new PurchaseInitiationDiagnosticsStepExecutor(
                GetRequiredService<IPurchaseManager>(),
                _purchaseInitiationDiagnosticsStepParams
            );
            var purchaseInitiationDiagnosticsStepChecker = new PurchaseInitiationDiagnosticsStepChecker(
                GetRequiredService<IUnitOfWorkFactory>(),
                GetRequiredService<IOngoingPurchaseActivityManager>()
            );
            var purchaseInitiationDiagnosticsStepCleaner = new PurchaseInitiationDiagnosticsStepCleaner(
                GetRequiredService<IOngoingPurchaseActivityManager>(),
                GetRequiredService<IUnitOfWorkFactory>(),
                GetRequiredService<IPosRemoteControllerFactory>()
            );

            return new DiagnosticsStep(
                diagnosticsStepInfo,
                diagnosticsStepPreparer,
                purchaseInitiationDiagnosticsStepExecutor,
                purchaseInitiationDiagnosticsStepChecker,
                purchaseInitiationDiagnosticsStepCleaner
            );
        }
    }
}