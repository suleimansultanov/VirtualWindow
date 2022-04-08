using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Constants;
using NasladdinPlace.Dtos.Purchase;
using Refit;

namespace NasladdinPlace.Api.Client.Rest.Api
{
    public interface IPurchasesApi : IApi
    {
        [Delete("/api/plants/active/doors")]
        [Headers(Headers.Authorization)]
        Task<string> InitiatePurchaseCompletionAsync();

        [Post("/api/plants/active/anotherDoor")]
        [Headers(Headers.Authorization)]
        Task<string> ContinuePurchaseAsync();
        
        [Post("/api/purchases")]
        [Headers(Headers.Authorization)]
        Task<PurchaseInitiationResultDto> InitiatePurchaseAsync([Body] PurchaseInitiationRequestDto initiationRequestDto);
    }
}