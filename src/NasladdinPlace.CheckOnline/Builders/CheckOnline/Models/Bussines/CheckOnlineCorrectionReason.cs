using System;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    /// Основание для коррекции
    /// </summary>
    public class CheckOnlineCorrectionReason : IOnlineCashierCorrectionReason
    {
        /// <summary>
        /// Наименование документа 
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Дата документа
        /// </summary>
        public DateTime DocumentDateTime { get; set; }
    }
}
