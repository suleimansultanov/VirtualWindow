using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    /// <summary>
    ///     Транспортная модель ответа chekonline
    /// </summary>
    public class CheckOnlineResponseTransport
    {
        /// <summary>
        ///     Ошибка запроса
        /// </summary>
        public string RequestError { get; set; }

        /// <summary>
        ///     Идентификатор клиента
        /// </summary>
        [JsonProperty("ClientId")]
        public string ClientId { get; set; }

        /// <summary>
        ///     Дата и время формирований документа
        /// </summary>
        [JsonProperty("Date")]
        public CheckOnlineResponseTransportDateTime Date { get; set; }

        /// <summary>
        ///     Идентификация устройства, обработавшего запрос
        /// </summary>
        [JsonProperty("Device")]
        public CheckOnlineResponseTransportDevice Device { get; set; }

        /// <summary>
        ///     Регистрационный номер устройства
        /// </summary>
        [JsonProperty("DeviceRegistrationNumber")]
        public string DeviceRegistrationNumber { get; set; }

        /// <summary>
        ///      Заводской номер устройства
        /// </summary>
        [JsonProperty("DeviceSerialNumber")]
        public string DeviceSerialNumber { get; set; }

        /// <summary>
        ///     Номер чека
        /// </summary>
        [JsonProperty("DocNumber")]
        public int DocNumber { get; set; }

        /// <summary>
        ///     Тип документа
        /// </summary>
        [JsonProperty("DocumentType")]
        public int DocumentType { get; set; }

        /// <summary>
        ///     Номер фискального накопителя, в котором сформирован документ
        /// </summary>
        [JsonProperty("FNSerialNumber")]
        public string FnSerialNumber { get; set; }

        /// <summary>
        ///     Номер фискального документа
        /// </summary>
        [JsonProperty("FiscalDocNumber")]
        public long FiscalDocNumber { get; set; }

        /// <summary>
        ///      Фискальный признак документа
        /// </summary>
        [JsonProperty("FiscalSign")]
        public long FiscalSign { get; set; }

        /// <summary>
        ///      Итог чека
        /// </summary>
        [JsonProperty("GrandTotal")]
        public long GrandTotal { get; set; }

        /// <summary>
        ///     Операция
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; set; }

        /// <summary>
        ///     QR-код чека
        /// </summary>
        [JsonProperty("QR")]
        public string QrCode { get; set; }
        
        /// <summary>
        ///     Уникальный ID запроса
        /// </summary>
        [JsonProperty("RequestId")]
        public string RequestId { get; set; }
        
        /// <summary>
        ///     Ответ
        /// </summary>
        [JsonProperty("Response")]
        public CheckOnlineResponseTransportResponse Response { get; set; }

        /// <summary>
        ///     Результат
        /// </summary>
        [JsonProperty("Responses")]
        public CheckOnlineResponseTransportResponses[] Responses { get; set; }
    }
}