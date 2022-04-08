using System;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public class ChangePhoneNumberInfo
    {
        public string PhoneNumber { get; }
        public string VerificationCode { get; }

        public ChangePhoneNumberInfo(string phoneNumber, string verificationCode)
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