using System.Runtime.Serialization;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;

namespace FirebaseCloudMessagingClient.Rest.Dtos.PushNotification
{
    [DataContract]
    public class PushNotificationDto
    {
        [DataMember(Name = "title")]
        public string Title { get; }
        
        [DataMember(Name = "body")]
        public string Body { get; }

        public PushNotificationDto(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public PushNotificationDto(PushNotificationContent pushNotificationContent)
        {
            Title = pushNotificationContent.Title;
            Body = pushNotificationContent.Body;
        }
    }
}