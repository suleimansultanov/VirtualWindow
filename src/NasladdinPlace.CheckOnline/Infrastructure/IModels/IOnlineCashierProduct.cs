namespace NasladdinPlace.CheckOnline.Infrastructure.IModels
{
    /// <summary>
    ///     Купленный продукт
    /// </summary>
    public interface IOnlineCashierProduct
    {
        /// <summary>
        ///     Название
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Сумма к оплате
        /// </summary>
        decimal Amount { get; set; }

        /// <summary>
        ///     Количество
        /// </summary>
        int Count { get; set; }
    }
}