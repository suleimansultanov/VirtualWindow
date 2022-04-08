using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.Client.Rest.RequestExecutor;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;

namespace NasladdinPlace.Api.Client.Rest.Client
{
    public class RestClient : IRestClient
    {
        private readonly string _baseApiUrl;
        private readonly IRequestExecutorFactory _requestExecutorFactory;
        private readonly IAuthTokenRetriever _authTokenRetriever;

        public RestClient(
            string baseApiUrl,
            IRequestExecutorFactory requestExecutorFactory,
            IAuthTokenRetriever authTokenRetriever)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl));
            if (requestExecutorFactory == null)
                throw new ArgumentNullException(nameof(requestExecutorFactory));
            if (authTokenRetriever == null)
                throw new ArgumentNullException(nameof(authTokenRetriever));
            
            _baseApiUrl = baseApiUrl;
            _requestExecutorFactory = requestExecutorFactory;
            _authTokenRetriever = authTokenRetriever;
        }

        public RequestExecutor<TApi> ForApi<TApi>() where TApi: IApi
        {
            return ProvideRequestExecutorForApi<TApi>();
        }

        public Task<RequestResponse<TResult>> PerformRequestAsync<TApi, TResult>(Func<TApi, Task<TResult>> requestFunc) 
            where TApi: IApi
        {
            if (requestFunc == null)
                throw new ArgumentNullException(nameof(requestFunc));

            var requestExecutor = ProvideRequestExecutorForApi<TApi>();
            
            return requestExecutor.PerformRequestAsync(requestFunc);
        }

        public event EventHandler OnUnauthorizedError;
        
        private void HandleUnauthorizedError(object sender, EventArgs eventArgs)
        {
            NotifyUnauthorizedError();
        }
        
        private void NotifyUnauthorizedError()
        {
            OnUnauthorizedError?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void SubscribeForUnauthorizedErrorEvent(IUnauthorizedErrorPublisher unauthorizedErrorPublisher)
        {
            unauthorizedErrorPublisher.OnUnauthorizedError -= HandleUnauthorizedError;
            unauthorizedErrorPublisher.OnUnauthorizedError += HandleUnauthorizedError;
        }
        
        private async Task<string> ProvideAuthTokenValue() {
            var authToken = await _authTokenRetriever.RetrieveAsync();
            return authToken.Value;
        }

        private RequestExecutor<TApi> ProvideRequestExecutorForApi<TApi>() where TApi: IApi
        {
            var requestExecutor = _requestExecutorFactory.CreateForApi<TApi>(
                _baseApiUrl,
                ProvideAuthTokenValue
            );
            SubscribeForUnauthorizedErrorEvent(requestExecutor);
            return (RequestExecutor<TApi>) requestExecutor;
        }
    }
}