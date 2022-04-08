namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    /// Основание для коррекции
    /// </summary>
    public class CheckOnlineCorrectionRequestTransportReason
    {
        /// <summary>
        /// Наименование документа 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата документа
        /// </summary>
        public CheckOnlineResponseTransportDate Date { get; set; }
    }
}
