using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory
{
    public sealed class PosTokenServicesOptions
    {
        private string _tokenPrefix;
        private string _encryptionKey;

        public TimeSpan TokenValidityPeriod { get; set; }

        public string TokenPrefix
        {
            get => _tokenPrefix;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _tokenPrefix = value;
            }
        }

        public string EncryptionKey
        {
            get => _encryptionKey;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _encryptionKey = value;
            }
        }

        public TimeSpan PosTokenProviderCachePeriod { get; set; }

        public PosTokenServicesOptions()
        {
            TokenValidityPeriod = TimeSpan.FromMinutes(3);
            _tokenPrefix = string.Empty;
            EncryptionKey = Guid.NewGuid().ToString();
            PosTokenProviderCachePeriod = TimeSpan.FromMinutes(10);
        }

        public bool HasTokenPrefix => !string.IsNullOrWhiteSpace(TokenPrefix);
    }
}