using CloudPaymentsClient.Domain.Factories.PaymentService;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Payment.Services;
using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation
{
    public class PaymentCardConfirmationDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        private readonly PaymentCardConfirmationDiagnosticsStepParams _paymentCardConfirmationDiagnosticsStepParams;
        private readonly ServiceInfo _paymentServiceInfo;

        public PaymentCardConfirmationDiagnosticsStepFactory(
            IServiceScope serviceScope,
            PaymentCardConfirmationDiagnosticsStepParams paymentCardConfirmationDiagnosticsStepParams,
            ServiceInfo paymentServiceInfo)
            : base(serviceScope)
        {
            if (paymentCardConfirmationDiagnosticsStepParams == null)
                throw new ArgumentNullException(nameof(paymentCardConfirmationDiagnosticsStepParams));
            if (paymentServiceInfo == null)
                throw new ArgumentNullException(nameof(paymentServiceInfo));

            _paymentCardConfirmationDiagnosticsStepParams = paymentCardConfirmationDiagnosticsStepParams;
            _paymentServiceInfo = paymentServiceInfo;
        }

        public override DiagnosticsStep Create()
        {
            var paymentCardConfirmationStepInfo = new PaymentCardConfirmationDiagnosticsStepInfo();

            var paymentCardConfirmationInfo = new PaymentCardConfirmationRequest(
                "TEST USER",
                _paymentCardConfirmationDiagnosticsStepParams.PaymentCardCryptogram,
                _paymentCardConfirmationDiagnosticsStepParams.UserIpAddress,
                PaymentCardCryptogramSource.Common
            );

            var paymentCardConfirmationExecutor = new PaymentConfirmationDiagnosticsStepExecutor(
                GetRequiredService<IPaymentCardConfirmationService>(),
                paymentCardConfirmationInfo,
                CreateTestPaymentService()
            );

            var paymentCardConfirmationStepChecker = new PaymentCardConfirmationDiagnosticsStepChecker(
                GetRequiredService<IUnitOfWorkFactory>()
            );

            var paymentCardConfirmationStepCleaner = new PaymentCardConfirmationDiagnosticsStepCleaner(
                GetRequiredService<IUnitOfWorkFactory>()
            );

            return new DiagnosticsStep(
                paymentCardConfirmationStepInfo,
                new NoDiagnosticsStepPreparation(),
                paymentCardConfirmationExecutor,
                paymentCardConfirmationStepChecker,
                paymentCardConfirmationStepCleaner
            );
        }

        private IPaymentService CreateTestPaymentService()
        {
            var paymentServiceFactory = new CloudPaymentsesServiceFactory(_paymentServiceInfo);
            return paymentServiceFactory.CreatePaymentService();
        }
    }
}