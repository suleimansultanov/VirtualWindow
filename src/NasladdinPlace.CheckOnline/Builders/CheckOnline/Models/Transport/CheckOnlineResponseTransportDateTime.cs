using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineResponseTransportDateTime
    {
        [JsonProperty("Date")]
        public CheckOnlineResponseTransportDate Date { get; set; }

        [JsonProperty("Time")]
        public CheckOnlineResponseTransportTime Time { get; set; }
    }
}