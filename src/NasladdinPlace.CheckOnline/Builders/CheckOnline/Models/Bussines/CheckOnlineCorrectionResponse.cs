using System;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    /// Результат на запрос о создании чека коррекции
    /// </summary>
    public class CheckOnlineCorrectionResponse : IOnlineCashierResponse
    {
        /// <inheritdoc />
        /// <summary>
        ///     Успешный результат
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Ошибки
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        ///     Дата и время формирования документа
        /// </summary>
        public DateTime? DocumentDateTime { get; set; }

        /// <summary>
        ///     Текст чека
        /// </summary>
        public string ReceiptInfo { get; set; }

        /// <summary>
        ///     Фискальная информация
        /// </summary>
        public CheckOnlineResponseFiscalData FiscalData { get; set; }
    }
}
