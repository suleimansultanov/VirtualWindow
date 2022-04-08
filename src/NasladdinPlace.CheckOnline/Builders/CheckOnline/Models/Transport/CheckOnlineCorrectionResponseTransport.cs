using System.Collections.Generic;
using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    ///     Транспортная модель ответа chekonline на коррекцию чека
    /// </summary>
    public class CheckOnlineCorrectionResponseTransport
    {
        /// <summary>
        ///     Код ошибки
        /// </summary>
        [JsonProperty("Error")]
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Список всех ошибок
        /// </summary>
        [JsonProperty("ErrorMessages")]
        public List<string> ErrorMessages { get; set; }

        /// <summary>
        ///     Дата и время формирований документа
        /// </summary>
        [JsonProperty("Date")]
        public CheckOnlineResponseTransportDateTime Date { get; set; }

        /// <summary>
        ///     Номер фискального документа
        /// </summary>
        [JsonProperty("FiscalDocNumber")]
        public long FiscalDocNumber { get; set; }

        /// <summary>
        ///     Номер документа в смене по данным ФН
        /// </summary>
        [JsonProperty("DocNumber")]
        public int DocumentNumber { get; set; }

        /// <summary>
        ///     Тип документа
        /// </summary>
        [JsonProperty("DocumentType")]
        public int DocumentType { get; set; }

        /// <summary>
        ///      Фискальный признак документа
        /// </summary>
        [JsonProperty("FiscalSign")]
        public long FiscalSign { get; set; }

        /// <summary>
        ///     Фискальный документ
        /// </summary>
        [JsonProperty("FiscalDocument")]
        public CheckOnlineCorrectionResponseTransportFiscalDocument FiscalDocument { get; set; }
    }
}
