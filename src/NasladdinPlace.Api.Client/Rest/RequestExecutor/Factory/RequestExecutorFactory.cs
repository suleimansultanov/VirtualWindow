using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory
{
    public class RequestExecutorFactory : IRequestExecutorFactory
    {   
        public IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl) where TApi : IApi
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl)); 
            
            var requestExecutor = new RequestExecutor<TApi>(baseApiUrl);
            return requestExecutor;
        }

        public IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl, Func<Task<string>> getAuthHeader) where TApi : IApi
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl));
            
            var requestExecutor = new RequestExecutor<TApi>(baseApiUrl, getAuthHeader);
            return requestExecutor;
        }
    }
}