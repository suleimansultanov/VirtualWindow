using System.Threading.Tasks;
using FirebaseCloudMessagingClient.Rest.Dtos.FirebaseToken;
using Refit;

namespace FirebaseCloudMessagingClient.Rest.Api
{
    public interface IFirebaseTokenApi
    {
        [Get("/iid/info/{token}")]
        Task<FirebaseTokenInfoDto> GetTokenInfoAsync(
            [Header("Authorization")] string authorizationHeader, [AliasAs("token")] string token
        );
    }
}