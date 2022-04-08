using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts
{
    public interface IRequestExecutor<out TApi> : IUnauthorizedErrorPublisher where TApi: IApi
    {   
        Task<RequestResponse<TResult>> PerformRequestAsync<TResult>(Func<TApi, Task<TResult>> requestFunc);
    }
}