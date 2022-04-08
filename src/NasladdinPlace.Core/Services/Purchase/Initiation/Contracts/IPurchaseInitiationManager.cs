using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;

namespace NasladdinPlace.Core.Services.Purchase.Initiation.Contracts
{
    public interface IPurchaseInitiationManager
    {
        Task<PurchaseInitiationResult> InitiatePurchaseAsync(PurchaseInitiationParams purchaseInitiationParams);
    }
}