using System.Collections.Generic;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FirebaseCloudMessagingClient.Rest.Dtos.PushNotification
{
    public class PushNotificationDataDto
    {
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalPayload { get; }

        public PushNotificationDataDto(PushNotificationContent pushNotificationContent)
        {
            AdditionalPayload = new Dictionary<string, JToken>();
            
            foreach (var (key, value) in pushNotificationContent.AdditionalInfo)
            {
                AdditionalPayload[key] = JToken.FromObject(value);
            }
        }
    }
}