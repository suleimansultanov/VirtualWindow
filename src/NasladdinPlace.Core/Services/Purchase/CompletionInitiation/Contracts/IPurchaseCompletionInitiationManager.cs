using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.CompletionInitiation.Contracts
{
    public interface IPurchaseCompletionInitiationManager
    {
        Task<Result> InitiatePurchaseCompletionAsync(PurchaseOperationParams initiationParams);
    }
}