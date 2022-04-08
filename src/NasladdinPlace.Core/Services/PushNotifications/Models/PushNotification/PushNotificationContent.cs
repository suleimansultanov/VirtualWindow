using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification
{
    public class PushNotificationContent
    {
        public string Title { get; }
        public string Body { get; }
        public IDictionary<string, string> AdditionalInfo { get; }

        public PushNotificationContent(string title, string body)
        {
            Title = title;
            Body = body;
            AdditionalInfo = new Dictionary<string, string>();
        }
    }
}