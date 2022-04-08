using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Payment.Services;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation
{
    public class PaymentConfirmationDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPaymentCardConfirmationService _paymentCardConfirmationService;
        private readonly PaymentCardConfirmationRequest _paymentCardConfirmationRequest;
        private readonly IPaymentService _paymentService;

        public PaymentConfirmationDiagnosticsStepExecutor(
            IPaymentCardConfirmationService paymentCardConfirmationService,
            PaymentCardConfirmationRequest paymentCardConfirmationRequest,
            IPaymentService paymentService)
        {
            if (paymentCardConfirmationService == null)
                throw new ArgumentNullException(nameof(paymentCardConfirmationService));
            if (paymentCardConfirmationRequest == null)
                throw new ArgumentNullException(nameof(paymentCardConfirmationRequest));
            if (paymentService == null)
                throw new ArgumentNullException(nameof(paymentService));

            _paymentCardConfirmationService = paymentCardConfirmationService;
            _paymentCardConfirmationRequest = paymentCardConfirmationRequest;
            _paymentService = paymentService;
        }
        
        public async Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(PaymentConfirmationDiagnosticsStepExecutor)} must have a user.");

            var bankingCardConfirmationResult =
                await _paymentCardConfirmationService.TryConfirmPaymentCardViaPaymentServiceAsync(
                    _paymentService, context.User.Id, _paymentCardConfirmationRequest
                );
            
            return bankingCardConfirmationResult.ConfirmationStatus != PaymentCardConfirmationStatus.ConfirmationSucceeded 
                ? Result.Failure(bankingCardConfirmationResult.Error) 
                : Result.Success();
        }
    }
}