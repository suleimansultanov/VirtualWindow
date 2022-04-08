using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Client.Rest.Client.Contracts
{
    public interface IRestClient : IUnauthorizedErrorPublisher
    {
        RequestExecutor<TApi> ForApi<TApi>() where TApi : IApi;
        Task<RequestResponse<TResult>> PerformRequestAsync<TApi, TResult>(Func<TApi, Task<TResult>> requestFunc)
            where TApi : IApi;
    }
}