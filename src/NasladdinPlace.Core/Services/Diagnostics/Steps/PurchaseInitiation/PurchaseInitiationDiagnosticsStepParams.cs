using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseInitiation
{
    public class PurchaseInitiationDiagnosticsStepParams
    {
        public string QrCode { get; }

        public PurchaseInitiationDiagnosticsStepParams(string qrCode)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
                throw new ArgumentNullException(nameof(qrCode));
            
            QrCode = qrCode;
        }
    }
}