using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineResponseTransportDevice
    {
        /// <summary>
        ///     Заводской номер устройства
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Адрес устройства
        /// </summary>
        [JsonProperty("Address")]
        public string Address { get; set; }
    }
}