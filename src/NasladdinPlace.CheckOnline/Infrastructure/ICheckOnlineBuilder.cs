using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Infrastructure
{
    /// <summary>
    ///     Интерфейс для формирования чека в онлайн кассе
    /// </summary>
    public interface ICheckOnlineBuilder
    {
        /// <summary>
        ///     Получение чека
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель на создание чека</param>
        /// <param name="fiscalizationType">Тип фискализации (обычная, возврат прихода, коррекция)</param>
        IOnlineCashierResponse BuildCheck(IOnlineCashierAuth authData, IOnlineCashierRequest request, FiscalizationType fiscalizationType);

        /// <summary>
        ///     Получение чека коррекции
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель на создание чека коррекции</param>
        /// <returns></returns>
        IOnlineCashierResponse BuildCorrectionCheck(IOnlineCashierAuth authData, IOnlineCashierCorrectionRequest request);

        /// <summary>
        ///     Валидация модели
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель на создание чека</param>
        /// <param name="errors">Ошибки</param>
        bool ValidateRequest(IOnlineCashierAuth authData, IOnlineCashierRequest request, out string errors);

        /// <summary>
        ///     Валидация модели коррекции чека
        /// </summary>
        /// <param name="authData">Данные авторизации</param>
        /// <param name="request">Модель на создание чека</param>
        /// <param name="errors">Ошибки</param>
        bool ValidateCorrectionRequest(IOnlineCashierAuth authData, IOnlineCashierCorrectionRequest request, out string errors);
    }
}
