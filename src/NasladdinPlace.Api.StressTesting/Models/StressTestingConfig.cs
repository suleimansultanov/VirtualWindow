using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Api.StressTesting.Models
{
    public sealed class StressTestingConfig
    {
        private const int DefaultConcurrentRequestsNumber = 100;

        private int _concurrentRequestsNumber;
        
        public string BaseApiUrl { get; }
        public ICollection<UserCredentials> UsersCredentials { get; }

        public int ConcurrentRequestsNumber
        {
            get => _concurrentRequestsNumber;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), 
                        value, 
                        "Concurrent requests number must be greater than zero."
                    );
                _concurrentRequestsNumber = value;
            }
        }

        public StressTestingConfig(string baseApiUrl, ICollection<UserCredentials> usersCredentials) 
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl));
            if (usersCredentials == null)
                throw new ArgumentNullException(nameof(usersCredentials));
            if (!usersCredentials.Any())
                throw new ArgumentException($"{nameof(usersCredentials)} must have at least one item.");

            BaseApiUrl = baseApiUrl;
            UsersCredentials = usersCredentials.ToImmutableList();
            ConcurrentRequestsNumber = DefaultConcurrentRequestsNumber;
        }
    }
}