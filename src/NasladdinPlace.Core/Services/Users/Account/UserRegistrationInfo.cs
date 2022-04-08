using System;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public class UserRegistrationInfo
    {
        public string PhoneNumber { get; }

        public UserRegistrationInfo(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            PhoneNumber = phoneNumber;
        }
    }
}