using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    ///     Данные авторизации для ЧЕК кассы
    /// </summary>
    public class CheckOnlineAuth : IOnlineCashierAuth
    {
        /// <summary>
        ///     Адрес API
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        ///     Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Данные сертификата в Base64
        /// </summary>
        public string CertificateData { get; set; }

        /// <summary>
        ///     Пароль от сертификата
        /// </summary>
        public string CertificatePassword { get; set; }
    }
}