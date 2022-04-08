using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers
{
    /// <summary>
    ///     Провайдер для http запросов
    /// </summary>
    public interface ICheckOnlineRequestProvider
    {
        /// <summary>
        ///     Отправить запрос в chekonline
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="requestData">Модель запроса</param>
        CheckOnlineResponseTransport SendRequest(CheckOnlineAuth authData, CheckOnlineRequestTransport requestData);

        /// <summary>
        ///     Отправить запрос batch в chekonline
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="requestData">Модель запроса batch</param>
        CheckOnlineBatchResponseTransport SendRequest(CheckOnlineAuth authData, CheckOnlineBatchRequestTransport requestData);
    }
}