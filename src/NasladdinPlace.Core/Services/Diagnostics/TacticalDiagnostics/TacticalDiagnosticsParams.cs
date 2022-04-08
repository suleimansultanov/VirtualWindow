using NasladdinPlace.CloudPaymentsCore;
using System;

namespace NasladdinPlace.Core.Services.Diagnostics.TacticalDiagnostics
{
    public class TacticalDiagnosticsParams
    {
        public string PhoneNumber { get; }
        public string BankingCardCryptogram { get; }
        public string PosQrCode { get; }
        public string UserIpAddress { get; set; }
        public bool IsDevelopmentModeEnabled { get; set; }
        public ServiceInfo PaymentServiceInfo { get; set; }

        public TacticalDiagnosticsParams(
            string phoneNumber,
            string bankingCardCryptogram,
            string posQrCode,
            ServiceInfo paymentServiceInfo)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(bankingCardCryptogram))
                throw new ArgumentNullException(nameof(bankingCardCryptogram));
            if (string.IsNullOrWhiteSpace(posQrCode))
                throw new ArgumentNullException(nameof(posQrCode));
            if (paymentServiceInfo == null)
                throw new ArgumentNullException(nameof(paymentServiceInfo));

            PhoneNumber = phoneNumber;
            BankingCardCryptogram = bankingCardCryptogram;
            PosQrCode = posQrCode;
            UserIpAddress = "192.168.1.122";
            IsDevelopmentModeEnabled = false;
            PaymentServiceInfo = paymentServiceInfo;
        }
    }
}