using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.Continuation
{
    public interface IPurchaseContinuationManager
    {
        Task<Result> ContinuePurchaseAsync(PurchaseOperationParams continuationParams);
    }
}