using System.Runtime.Serialization;

namespace FirebaseCloudMessagingClient.Rest.Dtos.PushNotification
{
    [DataContract]
    public class PushNotificationRequestDto
    {
        [DataMember(Name = "to")]
        public string To { get; }

        [DataMember(Name = "notification")]
        public PushNotificationDto Notification { get; }
        
        [DataMember(Name = "data")]
        public PushNotificationDataDto Data { get; }
        
        public PushNotificationRequestDto(NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification.PushNotification pushNotification)
        {
            To = pushNotification.RecipientToken;
            Notification = new PushNotificationDto(pushNotification.Content);
            Data = new PushNotificationDataDto(pushNotification.Content);
        }
    }
}