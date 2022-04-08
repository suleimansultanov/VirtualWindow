using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Purchase.Completion.Contracts
{
    public interface IPurchaseCompletionManager
    {
        event EventHandler<PurchaseCompletionResult> PurchaseCompleted;

        Task<PurchaseCompletionResult> CompleteUnpaidPurchaseOfUserAsync(int userId, int posOperationId, int? paymentCardId = null);
        Task<PurchaseCompletionResult> CompleteUnpaidPurchaseOfUserAsync(IUnitOfWork unitOfWork, int userId, int posOperationId, int? paymentCardId = null);
        Task<PurchaseCompletionResult> CompleteLastPurchaseOfUserAsync(int userId);
        Task<PurchaseCompletionResult> CompleteLastPurchaseOfUserAsync(IUnitOfWork unitOfWork, int userId);
        Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllPurchasesOfUserAsync(int userId);
        Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllPurchasesOfUserAsync(IUnitOfWork unitOfWork, int userId);
        Task<PurchaseCompletionResult> CompletePurchaseByPosOperationIdAsync(
            int posOperationId, 
            int userId, 
            PosOperationTransactionType operationTransactionType = PosOperationTransactionType.RegularPurchase);
        Task<PurchaseCompletionResult> CompletePurchaseByPosOperationIdAsync(
            IUnitOfWork unitOfWork, 
            int posOperationId, 
            int userId, 
            PosOperationTransactionType operationTransactionType = PosOperationTransactionType.RegularPurchase);
        Task CompletePurchasesOfUsersAsync(IEnumerable<int> userIds);
    }
}