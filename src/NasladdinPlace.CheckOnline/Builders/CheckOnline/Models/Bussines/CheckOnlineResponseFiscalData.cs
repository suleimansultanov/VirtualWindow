namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines
{
    /// <summary>
    ///     Фискальная информация
    /// </summary>
    public class CheckOnlineResponseFiscalData
    {
        /// <summary>
        ///     Номер фискального документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     Номер фискального накопителя
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        ///     Фискальный признак документа
        /// </summary>
        public string Sign { get; set; }
    }
}