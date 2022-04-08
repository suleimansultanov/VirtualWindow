using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Models
{
    public class BankingCardConfirmationNotificationEventArgs : EventArgs
    {
        public static BankingCardConfirmationNotificationEventArgs ConfirmationFailed(
            ApplicationUser user, string error, string localizedError)
        {
            return new BankingCardConfirmationNotificationEventArgs(
                user, PaymentCardConfirmationStatus.ConfirmationFailed, error, localizedError
            );
        }

        public static BankingCardConfirmationNotificationEventArgs WithStatus(
            ApplicationUser user, PaymentCardConfirmationStatus confirmationStatus)
        {
            return new BankingCardConfirmationNotificationEventArgs(user, confirmationStatus, string.Empty, string.Empty);
        }
        
        public static BankingCardConfirmationNotificationEventArgs ConfirmationSucceeded(
            ApplicationUser user)
        {
            return WithStatus(user, PaymentCardConfirmationStatus.ConfirmationSucceeded);
        }
        
        public ApplicationUser User { get; }
        public PaymentCardConfirmationStatus ConfirmationStatus { get; }
        public string Error { get; }
        public string PaymentServiceLocalizedError { get; }

        private BankingCardConfirmationNotificationEventArgs(
            ApplicationUser user, 
            PaymentCardConfirmationStatus confirmationStatus,
            string error,
            string paymentServiceLocalizedError)
        {
            User = user;
            ConfirmationStatus = confirmationStatus;
            Error = error;
            PaymentServiceLocalizedError = paymentServiceLocalizedError;
        }
    }
}