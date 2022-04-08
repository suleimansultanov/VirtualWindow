using System;
using System.Collections.Generic;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    public class CheckOnlineRequest : IOnlineCashierRequest
    {
        /// <summary>
        ///     ID выставленного счета
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        ///     Электронный адрес покупателя
        /// </summary>
        public string ClientPhoneOrEmail { get; set; }

        /// <summary>
        ///     Продукты
        /// </summary>
        public List<IOnlineCashierProduct> Products { get; set; }

        /// <summary>
        ///     Код налога
        /// </summary>
        public int TaxCode { get; set; }
    }
}