namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    ///     Массив товарных позиций
    /// </summary>
    public class CheckOnlineRequestTransportLines
    {
        /// <summary>
        ///     Количество
        /// </summary>
        public long Qty { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        ///     Признак способа расчёта
        /// </summary>
        public int PayAttribute { get; set; }

        /// <summary>
        ///     Признак предмета расчета
        /// </summary>
        public int LineAttribute { get; set; }

        /// <summary>
        ///     Код налога
        /// </summary>
        public int TaxId { get; set; }

        /// <summary>
        ///     Наименование товарной позиции.
        ///     Кодировка CP866
        /// </summary>
        public string Description { get; set; }
    }
}