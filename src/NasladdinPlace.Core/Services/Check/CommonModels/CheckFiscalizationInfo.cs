using System;

namespace NasladdinPlace.Core.Services.Check.CommonModels
{
    public class CheckFiscalizationInfo
    {
        public string FiscalNumber { get; }
        public string Sign { get; }
        public DateTime? DateOfPurchase { get; }
        public string QrCodeLink { get; }
        public string QrCodeValue { get; }
        public string FiscalCheckLink { get; }

        public static readonly CheckFiscalizationInfo Empty = null;

        public CheckFiscalizationInfo(
            string fiscalNumber,
            string sign,
            DateTime? dateOfPurchase,
            string qrCodeLink,
            string qrCodeValue,
            string fiscalCheckLink)
        {
            if (string.IsNullOrEmpty(fiscalNumber))
                throw new ArgumentNullException(nameof(fiscalNumber));
            if (string.IsNullOrEmpty(sign))
                throw new ArgumentNullException(nameof(sign));
            if (!dateOfPurchase.HasValue)
                throw new ArgumentNullException(nameof(dateOfPurchase));
            if (string.IsNullOrEmpty(qrCodeLink))
                throw new ArgumentNullException(nameof(qrCodeLink));
            if (string.IsNullOrEmpty(qrCodeValue))
                throw new ArgumentNullException(nameof(qrCodeValue));
            if (string.IsNullOrWhiteSpace(fiscalCheckLink))
                throw new ArgumentNullException(nameof(fiscalCheckLink));

            FiscalNumber = fiscalNumber;
            Sign = sign;
            DateOfPurchase = dateOfPurchase;
            QrCodeLink = qrCodeLink;
            QrCodeValue = qrCodeValue;
            FiscalCheckLink = fiscalCheckLink;
        }
    }
}