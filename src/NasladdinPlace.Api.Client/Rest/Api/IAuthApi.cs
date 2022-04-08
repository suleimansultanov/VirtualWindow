using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using Refit;

namespace NasladdinPlace.Api.Client.Rest.Api
{
    public interface IAuthApi : IApi
    {
        [Post("/connect/token")]
        Task<AuthPayloadDto> LoginUserAsync(
            [Body(BodySerializationMethod.UrlEncoded)]
            LoginDto data
        );
    }
}