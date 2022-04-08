using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.Purchase.CompletionInitiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Continuation;
using NasladdinPlace.Core.Services.Purchase.Initiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Purchase.Manager.Contracts
{
    public interface IPurchaseManager
    {
        event EventHandler<PurchaseCompletionResult> PurchaseCompleted; 
        
        IPurchaseInitiationManager InitiationManager { get; }
        IPurchaseContinuationManager ContinuationManager { get; }
        IPurchaseCompletionInitiationManager CompletionInitiationManager { get; }
        IPurchaseCompletionManager CompletionManager { get; }
        
        Task<PurchaseInitiationResult> InitiateAsync(PurchaseInitiationParams initiationParams);
        Task<Result> ContinuePurchaseAsync(PurchaseOperationParams continuationParams);
        Task<Result> InitiateCompletionAsync(PurchaseOperationParams initiationParams);
        Task<PurchaseCompletionResult> CompleteLastUnpaidAsync(PurchaseOperationParams completionParams);
        Task<PurchaseCompletionResult> CompleteLastUnpaidAsync(IUnitOfWork unitOfWork, PurchaseOperationParams completionParams);
        Task<PurchaseCompletionResult> CompleteUnpaidPurchaseAsync(PurchaseOperationParams completionParams, int posOperationId, int? paymentCardId = null);
        Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllUnpaidAsync(PurchaseOperationParams completionParams);
    }
}