using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory
{
    public class CachedRequestExecutorFactory : IRequestExecutorFactory
    {   
        private readonly IRequestExecutorFactory _requestExecutorFactory;
        private readonly ConcurrentDictionary<ApiTypeAndBaseUrl, object> _requestExecutorByApiTypeAndBaseApiUrlDictionary;

        public CachedRequestExecutorFactory(IRequestExecutorFactory requestExecutorFactory)
        {
            if (requestExecutorFactory == null)
                throw new ArgumentNullException(nameof(requestExecutorFactory));
            
            _requestExecutorFactory = requestExecutorFactory;
            _requestExecutorByApiTypeAndBaseApiUrlDictionary = new ConcurrentDictionary<ApiTypeAndBaseUrl, object>();
        }
        
        public IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl) where TApi : IApi
        {
            var apiType = typeof(TApi);
            var typeAndBaseApiUrl = new ApiTypeAndBaseUrl(apiType, baseApiUrl);
            var requestExecutor = _requestExecutorByApiTypeAndBaseApiUrlDictionary.GetOrAdd(
                typeAndBaseApiUrl,
                _requestExecutorFactory.CreateForApi<TApi>(baseApiUrl)
            );
            return (RequestExecutor<TApi>) requestExecutor;
        }

        public IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl, Func<Task<string>> getAuthHeader) where TApi : IApi
        {
            var apiType = typeof(TApi);
            var typeAndBaseApiUrl = new ApiTypeAndBaseUrl(apiType, baseApiUrl);
            var requestExecutor = _requestExecutorByApiTypeAndBaseApiUrlDictionary.GetOrAdd(
                typeAndBaseApiUrl,
                _requestExecutorFactory.CreateForApi<TApi>(baseApiUrl, getAuthHeader)
            );
            return (RequestExecutor<TApi>) requestExecutor;
        }
    }
}