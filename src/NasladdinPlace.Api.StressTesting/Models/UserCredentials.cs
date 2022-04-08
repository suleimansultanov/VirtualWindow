using System;

namespace NasladdinPlace.Api.StressTesting.Models
{
    public sealed class UserCredentials
    {
        public string UserName { get; }
        public string Password { get; }

        public UserCredentials(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            UserName = userName;
            Password = password;
        }
    }
}