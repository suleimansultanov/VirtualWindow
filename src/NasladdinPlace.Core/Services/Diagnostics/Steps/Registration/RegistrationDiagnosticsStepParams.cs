using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.Registration
{
    public class RegistrationDiagnosticsStepParams
    {
        public string PhoneNumber { get; }
        public bool ShouldCheckForSendingSms { get; }

        public RegistrationDiagnosticsStepParams(string phoneNumber, bool shouldCheckForSendingSms)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            PhoneNumber = phoneNumber;
            ShouldCheckForSendingSms = shouldCheckForSendingSms;
        }
    }
}