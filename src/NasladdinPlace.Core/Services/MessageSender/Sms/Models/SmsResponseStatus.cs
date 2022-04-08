using System.Collections.Generic;
using NasladdinPlace.Core.Services.MessageSender.Sms.Enumerations;
using Newtonsoft.Json;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Models
{
    public class SmsResponseStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_code")]
        public SmsResponseStatusRequest StatusCode { get; set; }

        [JsonProperty("status_text")]
        public string StatusText { get; set; }

        [JsonProperty("sms")]
        public IDictionary<string, SmsMessageStatus> SmsStatuses { get; set; }

        [JsonProperty("balance")]
        public decimal? Balance { get; set; }

        public SmsResponseStatus()
        {
            SmsStatuses = new Dictionary<string, SmsMessageStatus>();
        }
    }
}