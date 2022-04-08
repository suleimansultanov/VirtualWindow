using System;

namespace NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models
{
    public sealed class AuthToken
    {
        public string Value { get; }
        public TimeSpan ExpirationInterval { get; }
        public DateTime DateCreated { get; }

        public AuthToken(string value, TimeSpan expirationInterval)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
            ExpirationInterval = expirationInterval;
            DateCreated = DateTime.UtcNow;
        }

        public bool IsExpired => DateCreated.Add(ExpirationInterval) < DateTime.UtcNow;
    }
}