using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Factory
{
    public interface IRequestExecutorFactory
    {
        IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl) where TApi: IApi;
        IRequestExecutor<TApi> CreateForApi<TApi>(string baseApiUrl, Func<Task<string>> getAuthHeader) where TApi: IApi;
    }
}