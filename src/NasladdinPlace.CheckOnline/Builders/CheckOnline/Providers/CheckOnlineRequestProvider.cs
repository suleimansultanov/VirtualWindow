using System;
using System.Security.Cryptography.X509Certificates;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;
using NasladdinPlace.CheckOnline.Tools;
using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers
{
    /// <summary>
    ///     Провайдер для http запросов
    /// </summary>
    public class CheckOnlineRequestProvider : ICheckOnlineRequestProvider
    {
        private readonly IHttpWebRequestProvider _httpWebRequest;

        public CheckOnlineRequestProvider(IHttpWebRequestProvider httpWebRequest)
        {
            _httpWebRequest = httpWebRequest;
        }

        /// <summary>
        ///     Отправить запрос в chekonline
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="requestData">Модель запроса</param>
        public CheckOnlineResponseTransport SendRequest(CheckOnlineAuth authData, CheckOnlineRequestTransport requestData)
        {
            var requestJson = JsonConvert.SerializeObject(requestData);
            var url = $"{authData.ServiceUrl}/fr/api/v2/Complex";

            bool success;
            var result = SendRequest(authData, requestJson, url, out success);
            if (!success)
                return new CheckOnlineResponseTransport {RequestError = result};

            return JsonConvert.DeserializeObject<CheckOnlineResponseTransport>(result);
        }

        /// <summary>
        ///     Отправить запрос batch в chekonline
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="requestData">Модель запроса batch</param>
        public CheckOnlineBatchResponseTransport SendRequest(CheckOnlineAuth authData, CheckOnlineBatchRequestTransport requestData)
        {
            var requestJson = JsonConvert.SerializeObject(requestData);
            var url = $"{authData.ServiceUrl}/fr/api/v2/Batch";

            bool success;
            var result = SendRequest(authData, requestJson, url, out success);
            if (!success)
                return new CheckOnlineBatchResponseTransport { RequestError = result };

            return JsonConvert.DeserializeObject<CheckOnlineBatchResponseTransport>(result);
        }

        private string SendRequest(CheckOnlineAuth authData, string requestJson, string url, out bool success)
        {
            var sertificateData = Convert.FromBase64String(authData.CertificateData);
            var certificate = new X509Certificate2(sertificateData, authData.CertificatePassword);
            
            var result = _httpWebRequest.SendPostJson(url, requestJson, certificate, out success);
            
            return result;
        }
    }
}