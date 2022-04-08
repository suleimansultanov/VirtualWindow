using NasladdinPlace.CheckOnline.Infrastructure.IModels;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    ///     Продукт
    /// </summary>
    public class CheckOnlineRequestProduct : IOnlineCashierProduct
    {
        /// <summary>
        ///     Наименование продукта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Сумма
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///     Количество
        /// </summary>
        public int Count { get; set; }
    }
}