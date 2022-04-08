using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Helpers;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using Refit;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor
{
    public class RequestExecutor<TApi> : IRequestExecutor<TApi> where TApi: IApi
    {   
        private readonly TApi _api;

        public RequestExecutor(string baseApiUrl)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl)) 
                throw new ArgumentNullException(nameof(baseApiUrl));

            _api = RestService.For<TApi>(baseApiUrl);
        }

        public RequestExecutor(string baseApiUrl, Func<Task<string>> getAuthHeader)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl)) 
                throw new ArgumentNullException(nameof(baseApiUrl));
            if (getAuthHeader == null)
                throw new ArgumentNullException(nameof(getAuthHeader));

            _api = RestService.For<TApi>(new HttpClient(new AuthenticatedHttpClientHandler(getAuthHeader))
            {
                BaseAddress = new Uri(baseApiUrl)
            });
        }

        public event EventHandler OnUnauthorizedError;

        public Task<RequestResponse<TResult>> PerformRequestAsync<TResult>(Func<TApi, Task<TResult>> requestFunc)
        {
            if (requestFunc == null)
                throw new ArgumentNullException(nameof(requestFunc));

            return PerformRequestAuxAsync(requestFunc);
        }

        private async Task<RequestResponse<TResult>> PerformRequestAuxAsync<TResult>(
            Func<TApi, Task<TResult>> requestFunc)
        {
            try
            {
                var result = await requestFunc(_api);

                return result == null
                    ? RequestResponse<TResult>.Undefined()
                    : RequestResponse<TResult>.Success(result);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.Unauthorized)
                    return RequestResponse<TResult>.Failure(ex);
                
                NotifyAboutUnauthorizedError();
                
                return RequestResponse<TResult>.Unauthorized();
            }
            catch (Exception ex)
            {
                return RequestResponse<TResult>.Failure(ex);
            }
        }

        private void NotifyAboutUnauthorizedError()
        {
            OnUnauthorizedError?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}