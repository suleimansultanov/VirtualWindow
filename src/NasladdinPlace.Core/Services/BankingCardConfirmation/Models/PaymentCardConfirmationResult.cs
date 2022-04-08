using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Models
{
    public class PaymentCardConfirmationResult
    {
        public static PaymentCardConfirmationResult ConfirmationSuccessful()
        {
            return new PaymentCardConfirmationResult(
                PaymentCardConfirmationStatus.ConfirmationSucceeded, string.Empty, null, string.Empty
            );
        }

        public static PaymentCardConfirmationResult Require3DsAuthorization(string form3DsHtml, Info3Ds info3Ds)
        {
            return new PaymentCardConfirmationResult(
                PaymentCardConfirmationStatus.Require3DsAuthorization, form3DsHtml, info3Ds, string.Empty
            );
        }

        public static PaymentCardConfirmationResult ConfirmationFailed(string error)
        {
            return new PaymentCardConfirmationResult(
                PaymentCardConfirmationStatus.ConfirmationFailed, string.Empty, null, error
            );
        }
        
        public PaymentCardConfirmationStatus ConfirmationStatus { get; }
        public string Form3DsHtml { get; }
        public Info3Ds Info3Ds { get; }
        public string Error { get; }

        private PaymentCardConfirmationResult(
            PaymentCardConfirmationStatus confirmationStatus,
            string form3DsHtml,
            Info3Ds info3Ds,
            string error)
        {
            ConfirmationStatus = confirmationStatus;
            Form3DsHtml = form3DsHtml;
            Info3Ds = info3Ds;
            Error = error;
        }
    }
}