using NasladdinPlace.Core.Enums;
using MobilePlatform = NasladdinPlace.Core.Enums.MobilePlatform;

namespace NasladdinPlace.Core.Services.PushNotifications.Models.Token
{
    public class PushNotificationTokenInfo
    {
        public Brand Brand { get; }
        
        public MobilePlatform MobilePlatform { get; }

        public PushNotificationTokenInfo(Brand brand, MobilePlatform mobilePlatform)
        {
            Brand = brand;
            MobilePlatform = mobilePlatform;
        }
    }
}