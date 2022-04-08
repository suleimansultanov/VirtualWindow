using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PointsOfSaleVersionVerification;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PosScreenResolutionVerification;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseCompletion;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseContinuation;
using NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation;
using NasladdinPlace.Core.Services.Diagnostics.Steps.Registration;

namespace NasladdinPlace.Core.Services.Diagnostics.TacticalDiagnostics
{
    public class TacticalDiagnosticsRunnerFactory : IDiagnosticsRunnerFactory
    {
        private readonly TacticalDiagnosticsParams _tacticalDiagnosticsParams;

        public TacticalDiagnosticsRunnerFactory(TacticalDiagnosticsParams tacticalDiagnosticsParams)
        {
            if (tacticalDiagnosticsParams == null)
                throw new ArgumentNullException(nameof(tacticalDiagnosticsParams));
            
            _tacticalDiagnosticsParams = tacticalDiagnosticsParams;
        }
        
        public IDiagnosticsRunner Create(IServiceScope serviceScope)
        {
            var diagnosticStepsFactories = GetDiagnosticsStepFactories(serviceScope);
            var diagnosticsSteps = diagnosticStepsFactories.Select(dsf => dsf.Create()).ToArray();
            
            return new DiagnosticsRunner(diagnosticsSteps);
        }

        private IEnumerable<IDiagnosticsStepFactory> GetDiagnosticsStepFactories(IServiceScope serviceScope)
        {
            return new[]
            {
                GetPointsOfSaleScreenResolutionCheckDiagnosticsStepFactory(serviceScope),
                GetPointsOfSaleVersionCheckDiagnosticsStepFactory(serviceScope),
                GetRegistrationDiagnosticsStepFactory(serviceScope),
                GetBankingCardConfirmationDiagnosticsStepFactory(serviceScope),
                GetPurchaseInitiationDiagnosticsStepFactory(serviceScope),
                GetPurchaseContinuationDiagnosticsStepFactory(serviceScope),
                GetPurchaseCompletionDiagnosticsStepFactory(serviceScope)
            };
        }

        private IDiagnosticsStepFactory GetRegistrationDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            var registrationDiagnosticsStepParams = new RegistrationDiagnosticsStepParams(
                phoneNumber: _tacticalDiagnosticsParams.PhoneNumber,
                shouldCheckForSendingSms: !_tacticalDiagnosticsParams.IsDevelopmentModeEnabled
            );
            
            return new RegistrationDiagnosticsStepFactory(serviceScope, registrationDiagnosticsStepParams);
        }

        private IDiagnosticsStepFactory GetBankingCardConfirmationDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            var bankingCardConfirmationDiagnosticsStepParams = new PaymentCardConfirmationDiagnosticsStepParams(
                _tacticalDiagnosticsParams.BankingCardCryptogram,
                _tacticalDiagnosticsParams.UserIpAddress
            );

            return new PaymentCardConfirmationDiagnosticsStepFactory(
                serviceScope,
                bankingCardConfirmationDiagnosticsStepParams,
                _tacticalDiagnosticsParams.PaymentServiceInfo
            );
        }

        private IDiagnosticsStepFactory GetPurchaseInitiationDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            var purchaseInitiationDiagnosticStepParams = new PurchaseInitiationDiagnosticsStepParams(
                _tacticalDiagnosticsParams.PosQrCode
            );
            return new PurchaseInitiationDiagnosticsStepFactory(serviceScope, purchaseInitiationDiagnosticStepParams);
        }

        private static IDiagnosticsStepFactory GetPurchaseContinuationDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            return new PurchaseContinuationDiagnosticsStepFactory(serviceScope);
        }

        private static IDiagnosticsStepFactory GetPurchaseCompletionDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            return new PurchaseCompletionDiagnosticsStepFactory(serviceScope);
        }

        private static IDiagnosticsStepFactory GetPointsOfSaleVersionCheckDiagnosticsStepFactory(
            IServiceScope serviceScope)
        {
            return new PointsOfSaleVersionCheckDiagnosticsStepFactory(serviceScope);
        }

        private static IDiagnosticsStepFactory GetPointsOfSaleScreenResolutionCheckDiagnosticsStepFactory(
            IServiceScope serviceScope)
        {
            return new PosScreenResolutionDiagnosticsStepFactory(serviceScope);
        }
    }
}