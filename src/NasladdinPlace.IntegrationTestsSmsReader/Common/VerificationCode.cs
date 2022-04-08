using System;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public sealed class VerificationCode
    {
        public VerificationCodeType Type { get; }
        public string Value { get; }

        public VerificationCode(VerificationCodeType type, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type} {Value}";
        }
    }
}