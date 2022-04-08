using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineResponseTransportResponses
    {
        /// <summary>
        ///     Операция
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; set; }

        /// <summary>
        ///     Ответ
        /// </summary>
        [JsonProperty("Response")]
        public CheckOnlineResponseTransportResponse Response { get; set; }
    }
}