using System;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public class InboxCredentials
    {
        public string UserName { get; }
        public string Password { get; }

        public InboxCredentials(string userName, string password)
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