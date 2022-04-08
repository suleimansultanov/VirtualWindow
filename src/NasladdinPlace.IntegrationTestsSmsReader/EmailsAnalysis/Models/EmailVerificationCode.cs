using System;
using NasladdinPlace.IntegrationTestsSmsReader.Common;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.Models
{
    public sealed class EmailVerificationCode
    {
        public Email Email { get; }
        public VerificationCode VerificationCode { get; }

        public EmailVerificationCode(Email email, VerificationCode verificationCode)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));
            if (verificationCode == null)
                throw new ArgumentNullException(nameof(verificationCode));

            Email = email;
            VerificationCode = verificationCode;
        }

        public override string ToString()
        {
            return $"{Email}\n" +
                VerificationCode;
        }
    }
}