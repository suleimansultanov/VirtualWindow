namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    ///     Результат онлайн кассы
    /// </summary>
    public interface IOnlineCashierResponse
    {
        /// <summary>
        ///     Успешный результат
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        ///     Ошибки
        /// </summary>
        string Errors { get; set; }
    }
}