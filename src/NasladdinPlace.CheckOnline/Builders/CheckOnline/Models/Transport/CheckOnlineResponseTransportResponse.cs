using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineResponseTransportResponse
    {
        /// <summary>
        ///      Код ошибки
        /// </summary>
        [JsonProperty("Error")]
        public int Error { get; set; }

        /// <summary>
        ///     Список сообщений, сформированных устройством при обработке запроса
        /// </summary>
        [JsonProperty("ErrorMessages")]
        public string[] ErrorMessages { get; set; }

        /// <summary>
        ///     Полный текст чека
        /// </summary>
        [JsonProperty("Text")]
        public string ReceiptFullText { get; set; }
    }
}