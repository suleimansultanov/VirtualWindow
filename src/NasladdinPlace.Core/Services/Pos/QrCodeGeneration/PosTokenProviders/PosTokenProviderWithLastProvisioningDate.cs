using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class PosTokenProviderWithLastProvisioningDate
    {
        public DateTime ProvisioningDate { get; }
        public IPosTokenProvider PosTokenProvider { get; }

        public PosTokenProviderWithLastProvisioningDate(IPosTokenProvider posTokenProvider)
        {
            if (posTokenProvider == null)
                throw new ArgumentNullException(nameof(posTokenProvider));

            ProvisioningDate = DateTime.UtcNow;
            PosTokenProvider = posTokenProvider;
        }

        public bool IsProvisioningDateExpired(TimeSpan expirationPeriod)
        {
            return DateTime.UtcNow - expirationPeriod > ProvisioningDate;
        }
    }
}