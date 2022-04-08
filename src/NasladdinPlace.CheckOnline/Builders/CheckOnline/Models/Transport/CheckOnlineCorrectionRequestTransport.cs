namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    ///     Транспортная модель запроса chekonline для коррекции чека
    /// </summary>
    public class CheckOnlineCorrectionRequestTransport
    {
        /// <summary>
        /// Пароль доступа (1-27 - кассиры,
        ///                 28 - старший кассир,
        ///                 29 - старший администратор
        ///                 30 - системный администратор
        ///                 31 - супер администратор)
        /// </summary>
        public int Password { get; set; }

        /// <summary>
        ///     Сумма коррекции безналичным 
        /// </summary>
        public long NonCash { get; set; }

        /// <summary>
        ///     Тип документа
        /// </summary>
        public int DocumentType { get; set; }

        /// <summary>
        /// Тип коррекции
        /// </summary>
        public int CorrectionType { get; set; }

        /// <summary>
        /// Основание для коррекции
        /// </summary>
        public CheckOnlineCorrectionRequestTransportReason Reason { get; set; }
    }
}
