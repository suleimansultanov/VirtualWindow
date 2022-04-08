using System.Runtime.Serialization;

namespace FirebaseCloudMessagingClient.Rest.Dtos.FirebaseToken
{
    [DataContract]
    public class FirebaseTokenInfoDto
    {
        [DataMember(Name = "applicationVersion")]
        public string ApplicationVersion { get; set; }

        [DataMember(Name = "application")]
        public string Application { get; set; }
        
        [DataMember(Name = "platform")]
        public string Platform { get; set; }
    }
}