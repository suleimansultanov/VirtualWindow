using System;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public class VerificationPhoneNumberInfo
    {
        public string PhoneNumber { get; }
        public string VerificationCode { get; }

        public VerificationPhoneNumberInfo(string phoneNumber, string verificationCode)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));
            if (string.IsNullOrEmpty(verificationCode))
                throw new ArgumentNullException(nameof(verificationCode));
            
            PhoneNumber = phoneNumber;
            VerificationCode = verificationCode;
        }
    }
}