using System.Collections.Generic;
using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineBatchResponseTransport
    {
        /// <summary>
        ///     Ошибка запроса
        /// </summary>
        public string RequestError { get; set; }

        /// <summary>
        ///     Идентификатор запроса
        /// </summary>
        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        /// <summary>
        ///     Код ошибки последней выполненной команды пакетаы
        /// </summary>
        [JsonProperty("Error")]
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Список всех ExchangeError и ErrorMessages команд пакета
        /// </summary>
        [JsonProperty("ErrorMessages")]
        public List<string> ErrorMessages { get; set; }

        /// <summary>
        /// Результаты выполнения комманд
        /// </summary>
        [JsonProperty("Responses")]
        public List<CheckOnlineBatchResponseTransportCommand> Responses { get; set; }
    }
}
