using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineResponseTransportTime
    {
        [JsonProperty("Hour")]
        public int Hour { get; set; }

        [JsonProperty("Minute")]
        public int Minute { get; set; }

        [JsonProperty("Second")]
        public int Second { get; set; }
    }
}