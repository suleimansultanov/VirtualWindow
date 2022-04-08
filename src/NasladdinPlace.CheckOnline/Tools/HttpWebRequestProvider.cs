using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace NasladdinPlace.CheckOnline.Tools
{
    /// <summary>
    ///     HttpWeb запросы
    /// </summary>
    public class HttpWebRequestProvider : IHttpWebRequestProvider
    {
        /// <summary>
        ///     Отправить POST запрос с JSON данными
        /// </summary>
        /// <param name="url">УРЛ</param>
        /// <param name="jsonData">Данные</param>
        /// <param name="success">Статус</param>
        public string SendPostJson(string url, string jsonData, out bool success)
        {
            return SendPostJson(url, jsonData, null, out success);
        }

        /// <summary>
        ///     Отправить POST запрос с JSON данными
        /// </summary>
        /// <param name="url">УРЛ</param>
        /// <param name="jsonData">Данные</param>
        /// <param name="success">Статус</param>
        /// <param name="certificate">Сертификат</param>
        public string SendPostJson(string url, string jsonData, X509Certificate2 certificate, out bool success)
        {
            try
            {
                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                if (certificate != null)
                    webRequest.ClientCertificates.Add(certificate);

                var streamWriter = new StreamWriter(webRequest.GetRequestStream());
                streamWriter.Write(jsonData);
                streamWriter.Close();

                using (var response = (HttpWebResponse) webRequest.GetResponse())
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var decodeString = HttpUtility.HtmlDecode(responseString);

                    success = true;
                    return decodeString;
                }                
            }
            catch (WebException ex)
            {
                var exMessage = ex.ToString();
                if (ex.Response != null)
                {
                    using (var responseReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        exMessage = responseReader.ReadToEnd();
                    }
                }
                success = false;
                return exMessage;
            }
            catch (Exception e)
            {
                success = false;
                return e.ToString();
            }
        }

        /// <summary>
        /// Выполнить запрос с Basic авторизацией.
        /// </summary>
        /// <param name="uri">Урл.</param>
        /// <param name="username">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <param name="success">Признак успешности.</param>
        /// <returns>Результат выполнения запроса.</returns>
        public string HttpRequestByBasicAuth(string uri, string username, string password, out bool success)
        {

            try
            {
                var authInfo = username + ":" + password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                var request = (HttpWebRequest)WebRequest.Create(uri);

                request.Headers["Authorization"] = "Basic " + authInfo;

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    success = true;
                    return sr.ReadToEnd();

                }

            }
            catch (Exception ex)
            {
                success = false;
                return ex.ToString();
            }
        }
    }
}
