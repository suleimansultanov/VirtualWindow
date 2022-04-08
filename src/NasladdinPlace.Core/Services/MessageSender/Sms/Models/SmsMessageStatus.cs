using NasladdinPlace.Core.Services.MessageSender.Sms.Enumerations;
using Newtonsoft.Json;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Models
{
    public class SmsMessageStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_code")]
        public SmsResponseStatusRequest StatusCode { get; set; }

        [JsonProperty("status_text")]
        public string StatusText { get; set; }

        [JsonProperty("sms_id")]
        public string SmsId { get; set; }
    }
}
