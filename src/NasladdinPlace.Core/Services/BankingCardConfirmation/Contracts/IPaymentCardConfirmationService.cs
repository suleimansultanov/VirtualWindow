using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Payment.Models;
using IPaymentService = NasladdinPlace.Payment.Services.IPaymentService;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts
{
    public interface IPaymentCardConfirmationService
    {
        event EventHandler<BankingCardConfirmationNotificationEventArgs> ConfirmationProgressUpdated;

        Task<PaymentCardConfirmationResult> TryConfirmPaymentCardViaPaymentServiceAsync(
            IPaymentService paymentService,
            int userId,
            PaymentCardConfirmationRequest confirmationRequest
        );
        
        Task<PaymentCardConfirmationResult> TryConfirmPaymentCardAsync(
            int userId, PaymentCardConfirmationRequest confirmationRequest
        );

        Task<PaymentCardConfirmationResult> CompletePaymentCardConfirmationAsync(
            int userId, Payment3DsCompletionRequest payment3DsCompletionRequest
        );
    }
}