using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PaymentCardConfirmation
{
    public class PaymentCardConfirmationDiagnosticsStepParams
    {
        public string PaymentCardCryptogram { get; }
        public string UserIpAddress { get; }

        public PaymentCardConfirmationDiagnosticsStepParams(string paymentCardCryptogram, string userIpAddress)
        {
            if (string.IsNullOrWhiteSpace(paymentCardCryptogram))
                throw new ArgumentException(nameof(paymentCardCryptogram));
            if (string.IsNullOrWhiteSpace(userIpAddress))
                throw new ArgumentException(nameof(userIpAddress));

            PaymentCardCryptogram = paymentCardCryptogram;
            UserIpAddress = userIpAddress;
        }
    }
}