using System.Threading.Tasks;
using FirebaseCloudMessagingClient.Rest.Dtos.PushNotification;
using Refit;

namespace FirebaseCloudMessagingClient.Rest.Api
{
    public interface IFirebaseCloudMessagingApi
    {
        [Post("/fcm/send")]
        Task<PushNotificationResponseDto> SendPushNotificationAsync(
            [Header("Authorization")] string authorizationHeader,
            [Body] PushNotificationRequestDto pushNotificationRequest
        );
    }
}