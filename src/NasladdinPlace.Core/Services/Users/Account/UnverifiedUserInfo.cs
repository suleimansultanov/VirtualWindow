using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public class UnverifiedUserInfo
    {
        public ApplicationUser User { get; }
        public string VerificationCode { get; }

        public UnverifiedUserInfo(ApplicationUser user, string verificationCode)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(verificationCode))
                throw new ArgumentNullException(nameof(verificationCode));
            
            User = user;
            VerificationCode = verificationCode;
        }
    }
}