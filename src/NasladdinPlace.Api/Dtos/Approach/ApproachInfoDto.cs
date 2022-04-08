using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Approach
{
    [DataContract]
    public class ApproachInfoDto
    {
        [DataMember(Name = "uid")]
        public string DeviceId { get; set; }

        [DataMember(Name = "url")]
        public string DeviceUrl { get; set; }

        [DataMember(Name = "ssid")]
        public string Ssid { get; set; }

        [DataMember(Name = "pswd")]
        public string Password { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "timeout")]
        public string Timeout { get; set; }

        [DataMember(Name = "events")]
        public ApproachEventDto[] Events { get; set; }
    }
}