namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    ///     Данные авторизации для онлайн кассы
    /// </summary>
    public interface IOnlineCashierAuth
    {
        /// <summary>
        ///     Адрес API
        /// </summary>
        string ServiceUrl { get; set; }

        /// <summary>
        ///     Логин
        /// </summary>
        string Login { get; set; }

        /// <summary>
        ///     Пароль
        /// </summary>
        string Password { get; set; }
    }
}