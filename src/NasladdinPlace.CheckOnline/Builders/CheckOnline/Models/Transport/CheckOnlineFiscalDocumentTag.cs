using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineFiscalDocumentTag
    {
        [JsonProperty("TagID")]
        public int TagId { get; set; }

        [JsonProperty("TagType")]
        public string TagType { get; set; }

        [JsonProperty("Value")]
        public object Value { get; set; }
    }
}
