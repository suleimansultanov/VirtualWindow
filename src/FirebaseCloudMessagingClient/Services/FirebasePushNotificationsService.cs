using FirebaseCloudMessagingClient.Rest.Api;
using FirebaseCloudMessagingClient.Rest.Dtos.PushNotification;
using FirebaseCloudMessagingClient.Rest.Mappers;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.PushNotifications;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using NasladdinPlace.Core.Services.PushNotifications.Models.Token;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace FirebaseCloudMessagingClient.Services
{
    public class FirebasePushNotificationsService : IPushNotificationsService
    {
        private readonly string _firebaseAuthHeader;
        private readonly IFirebaseCloudMessagingApi _firebaseCloudMessagingApi;
        private readonly IFirebaseTokenApi _firebaseTokenApi;
        private readonly INotificationsLogger _notificationsLogger;
        private readonly PushNotificationTokenInfoMapper _pushNotificationTokenInfoMapper;

        public FirebasePushNotificationsService(
            string firebaseApiKey,
            IFirebaseCloudMessagingApi firebaseCloudMessagingApi,
            IFirebaseTokenApi firebaseTokenApi,
            INotificationsLogger notificationsLogger)
        {
            _firebaseAuthHeader = $"key={firebaseApiKey}";
            _firebaseCloudMessagingApi = firebaseCloudMessagingApi;
            _firebaseTokenApi = firebaseTokenApi;
            _notificationsLogger = notificationsLogger;
            _pushNotificationTokenInfoMapper = new PushNotificationTokenInfoMapper();
        }
        
        public async Task<PushNotificationDeliveryResult> SendNotificationAsync(PushNotification pushNotification)
        {   
            try
            {
                var pushNotificationRequestDto = new PushNotificationRequestDto(pushNotification);

                var pushNotificationResponseDto =
                    await _firebaseCloudMessagingApi.SendPushNotificationAsync(
                        _firebaseAuthHeader,
                        pushNotificationRequestDto
                    );

                if (pushNotificationResponseDto.MessagesDelivered <= 0)
                    return PushNotificationDeliveryResult.Failure("Push notifications delivery failed.");

                await _notificationsLogger.LogPushAsync(pushNotification);

                return PushNotificationDeliveryResult.Success();

            }
            catch (Exception ex)
            {
                return PushNotificationDeliveryResult.Failure(
                    $"Messages failed: {ex}"
                );
            }
        }

        public async Task<ValueResult<PushNotificationTokenInfo>> GetTokenInfoAsync(string token)
        {
            try
            {
                var firebaseTokenInfoDto = await _firebaseTokenApi.GetTokenInfoAsync(_firebaseAuthHeader, token);
                
                var pushNotificationTokenInfo = _pushNotificationTokenInfoMapper.Transform(firebaseTokenInfoDto);

                return ValueResult<PushNotificationTokenInfo>.Success(pushNotificationTokenInfo);
            }
            catch (Exception ex)
            {
                return ValueResult<PushNotificationTokenInfo>.Failure($"Message failed: {ex}.");
            }
        }
    }
}