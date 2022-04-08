using System.Threading.Tasks;
using NasladdinPlace.Core.Services.WebClient.Models;

namespace NasladdinPlace.Core.Services.WebClient.Contracts
{
    public interface IJsonWebClient
    {
        Task<RequestResult<T>> PerformGetRequestAsync<T>(string url) where T : class;
        Task<RequestResult<T>> PerformAuthorizedGetRequestAsync<T>(string url, string authorizationHeaderValue) where T : class;
        Task<RequestResult<T>> PerformPostRequestAsync<T>(string url, object body) where T: class;
        Task<RequestResult<T>> PerformAuthorizedPostRequestAsync<T>(string url, object body, string authorizationHeaderValue) where T: class;
    }
}
