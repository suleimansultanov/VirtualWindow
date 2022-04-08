using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineBatchResponseTransportCommand
    {
        /// <summary>
        /// Адрес выполненой команды
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; set; }

        /// <summary>
        /// Ошибка выполнения команды
        /// </summary>
        [JsonProperty("ExchangeError")]
        public string ExchangeError { get; set; }

        /// <summary>
        /// Тело ответа
        /// </summary>
        [JsonProperty("Response")]
        public JObject Response { get; set; }
    }
}
