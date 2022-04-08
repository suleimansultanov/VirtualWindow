using System.Runtime.Serialization;

namespace FirebaseCloudMessagingClient.Rest.Dtos.PushNotification
{
    [DataContract]
    public class PushNotificationResponseDto
    {
        [DataMember(Name = "success")]
        public int MessagesDelivered { get; set; }
        
        [DataMember(Name = "messagesFailed")]
        public int MessagesFailed { get; set; }
    }
}