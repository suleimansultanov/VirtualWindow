using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public class VerifiedUserInfo
    {
        public ApplicationUser User { get; }
        public string GeneratedPassword { get; }
        public UserRegistrationStatus PreviousRegistrationStatus { get; set; }

        public VerifiedUserInfo(
            ApplicationUser user,
            string generatedPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(generatedPassword))
                throw new ArgumentNullException(nameof(generatedPassword));
            
            User = user;
            GeneratedPassword = generatedPassword;
            PreviousRegistrationStatus = UserRegistrationStatus.None;
        }
    }
}