using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.WebClient.Contracts;
using NasladdinPlace.Core.Services.WebClient.Models;
using Newtonsoft.Json;

namespace NasladdinPlace.Core.Services.WebClient
{
    public class JsonWebClient : IJsonWebClient
    {
        public async Task<RequestResult<T>> PerformGetRequestAsync<T>(string url) where T : class
        {
            return await PerformGetRequestAuxAsync<T>(url, httpRequestMessage => { });
        }

        public async Task<RequestResult<T>> PerformAuthorizedGetRequestAsync<T>(string url, string authorizationHeaderValue) where T : class
        {
            return await PerformGetRequestAuxAsync<T>(url, httpRequestMessage =>
            {
                httpRequestMessage.Headers.Add(HttpRequestHeader.Authorization.ToString(), authorizationHeaderValue);
            });
        }

        public async Task<RequestResult<T>> PerformPostRequestAsync<T>(string url, object body) where T : class
        {
            return await PerformPostRequestAuxAsync<T>(url, body, httpContent => { });
        }

        public async Task<RequestResult<T>> PerformAuthorizedPostRequestAsync<T>(string url, object body, string authorizationHeaderValue) where T : class
        {
            return await PerformPostRequestAuxAsync<T>(url, body, httpContent =>
            {
                httpContent.Headers.Add(HttpRequestHeader.Authorization.ToString(), authorizationHeaderValue);
            });
        }

        #region Helpers

        private static Task<RequestResult<T>> PerformGetRequestAuxAsync<T>(
            string url, Action<HttpRequestMessage> requestMessageConfigurationHandler) where T : class
        {
            return PerformRequestAsync(async httpClient =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessageConfigurationHandler(requestMessage);
                var response = await httpClient.SendAsync(requestMessage);
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedResponseBody = JsonConvert.DeserializeObject<T>(responseBody);
                return RequestResult<T>.Success(deserializedResponseBody);
            });
        }

        private static Task<RequestResult<T>> PerformPostRequestAuxAsync<T>(
            string url, object body, Action<HttpContent> httpContentConfigurationHandler) where T: class
        {
            return PerformRequestAsync(async httpClient =>
            {
                var jsonBody = JsonConvert.SerializeObject(body);
                var httpContent = new StringContent(jsonBody, Encoding.UTF8);
                httpContentConfigurationHandler(httpContent);
                var response = await httpClient.PostAsync(url, httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedResponseBody = JsonConvert.DeserializeObject<T>(responseBody);
                return RequestResult<T>.Success(deserializedResponseBody);
            });
        }

        private static async Task<RequestResult<T>> PerformRequestAsync<T>(Func<HttpClient, Task<RequestResult<T>>> requestHandler) where T : class
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await requestHandler(client);
                }
                catch (Exception ex)
                {
                    return RequestResult<T>.Failure(ex);
                }
            }
        }

        #endregion
    }
}
