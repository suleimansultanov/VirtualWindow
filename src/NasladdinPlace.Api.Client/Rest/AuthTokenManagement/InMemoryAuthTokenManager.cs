using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;

namespace NasladdinPlace.Api.Client.Rest.AuthTokenManagement
{
    public class InMemoryAuthTokenManager : IAuthTokenManager
    {
        private readonly object _authTokenLock = new object();

        private AuthToken _authToken;
        
        public Task<AuthToken> RetrieveAsync()
        {
            lock (_authTokenLock)
            {
                return Task.FromResult(_authToken);
            }
        }

        public Task UpdateAsync(AuthToken authToken)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));
            
            lock (_authTokenLock)
            {
                _authToken = authToken;
            }

            return Task.CompletedTask;
        }

        public Task RemoveAuthTokenAsync()
        {
            lock (_authTokenLock)
            {
                _authToken = null;
            }

            return Task.CompletedTask;
        }
    }
}