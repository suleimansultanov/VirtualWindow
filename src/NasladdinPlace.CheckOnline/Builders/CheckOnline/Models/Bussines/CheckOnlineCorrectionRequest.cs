using System;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    /// Запрос на создание чека коррекции
    /// </summary>
    public class CheckOnlineCorrectionRequest : IOnlineCashierCorrectionRequest
    {
        /// <summary>
        ///     ID выставленного счета
        /// </summary>
        public Guid InvoiceId { get; set; }


        /// <summary>
        /// Сумма коррекции
        /// </summary>
        public decimal CorrectionAmount { get; set; }

        /// <summary>
        /// Основание для коррекции 
        /// </summary>
        public IOnlineCashierCorrectionReason CorrectionReason { get; set; }

        /// <summary>
        ///     Код налога
        /// </summary>
        public int TaxCode { get; set; }
    }
}
