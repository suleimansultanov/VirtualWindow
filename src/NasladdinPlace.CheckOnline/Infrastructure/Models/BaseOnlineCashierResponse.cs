using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Infrastructure.Models
{
    /// <summary>
    ///     Базовый ответ онлайн кассы
    /// </summary>
    public class BaseOnlineCashierResponse : IOnlineCashierResponse
    {
        public bool IsSuccess { get; set; }
        public string Errors { get; set; }
    }
}