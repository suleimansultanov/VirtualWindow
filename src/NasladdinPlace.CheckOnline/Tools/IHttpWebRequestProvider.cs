using System.Security.Cryptography.X509Certificates;

namespace NasladdinPlace.CheckOnline.Tools
{
    /// <summary>
    ///     HttpWeb запросы
    /// </summary>
    public interface IHttpWebRequestProvider
    {
        /// <summary>
        ///     Отправить POST запрос с JSON данными
        /// </summary>
        /// <param name="url">УРЛ</param>
        /// <param name="jsonData">Данные</param>
        /// <param name="success">Статус</param>
        string SendPostJson(string url, string jsonData, out bool success);

        /// <summary>
        ///     Отправить POST запрос с JSON данными
        /// </summary>
        /// <param name="url">УРЛ</param>
        /// <param name="jsonData">Данные</param>
        /// <param name="success">Статус</param>
        /// <param name="certificate">Сертификат</param>
        string SendPostJson(string url, string jsonData, X509Certificate2 certificate, out bool success);

        /// <summary>
        /// Выполнить запрос с Basic авторизацией.
        /// </summary>
        /// <param name="uri">Урл.</param>
        /// <param name="username">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <param name="success">Признак успешности.</param>
        /// <returns>Результат выполнения запроса.</returns>
        string HttpRequestByBasicAuth(string uri, string username, string password, out bool success);
    }
}
