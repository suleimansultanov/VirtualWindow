using System;

namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    /// Основание для коррекции
    /// </summary>
    public interface IOnlineCashierCorrectionReason
    {
        /// <summary>
        /// Наименование документа 
        /// </summary>
        string DocumentName { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        string DocumentNumber { get; set; }

        /// <summary>
        /// Дата документа
        /// </summary>
        DateTime DocumentDateTime { get; set; }
    }
}
