using System;
using System.Collections.Generic;

namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    ///     Запрос для создания чека в онлайн кассе
    /// </summary>
    public interface IOnlineCashierRequest
    {
        /// <summary>
        ///     ID счета
        /// </summary>
        Guid InvoiceId { get; set; }

        /// <summary>
        ///     Email клиента
        /// </summary>
        string ClientPhoneOrEmail { get; set; }

        /// <summary>
        ///     Купленные продукты
        /// </summary>
        List<IOnlineCashierProduct> Products { get; set; }
    }
}