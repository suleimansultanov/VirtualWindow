using System;

namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    /// Запрос на создание чека коррекции
    /// </summary>
    public interface IOnlineCashierCorrectionRequest
    {
        /// <summary>
        ///     ID счета
        /// </summary>
        Guid InvoiceId { get; set; }

        /// <summary>
        /// Сумма коррекции
        /// </summary>
        decimal CorrectionAmount { get; set; }

        /// <summary>
        /// Основание для коррекции 
        /// </summary>
        IOnlineCashierCorrectionReason CorrectionReason { get; set; }
    }
}
