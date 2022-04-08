using System;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory;

namespace NasladdinPlace.Api.Client.Rest.Client.Factory
{
    public static class RestClientFactory
    {
        public static IRestClient Create(string baseApiUrl, IAuthTokenRetriever authTokenRetriever)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl));
            if (authTokenRetriever == null)
                throw new ArgumentNullException(nameof(authTokenRetriever));

            var requestExecutorFactory = new RequestExecutorFactory();
            var cachedRequestExecutorFactory = new CachedRequestExecutorFactory(requestExecutorFactory);
            
            return new RestClient(baseApiUrl, cachedRequestExecutorFactory, authTokenRetriever);
        }
    }
}