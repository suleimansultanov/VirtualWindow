using FirebaseCloudMessagingClient.Rest.Dtos.FirebaseToken;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.PushNotifications.Models.Token;
using System;
using MobilePlatform = NasladdinPlace.Core.Enums.MobilePlatform;

namespace FirebaseCloudMessagingClient.Rest.Mappers
{
    public class PushNotificationTokenInfoMapper
    {
        public PushNotificationTokenInfo Transform(FirebaseTokenInfoDto firebaseTokenInfoDto)
        {
            return new PushNotificationTokenInfo(Brand.Invalid, GetPlatformByString(firebaseTokenInfoDto.Platform));
        }

        private MobilePlatform GetPlatformByString(string platform)
        {
            var lowercasePlatform = platform.ToLower().Trim();
            switch (lowercasePlatform)
            {
                case "android":
                    return MobilePlatform.Android;
                case "ios":
                    return MobilePlatform.iOS;
                default:
                    throw new ArgumentException(nameof(platform), platform);
            }
        }
    }
}