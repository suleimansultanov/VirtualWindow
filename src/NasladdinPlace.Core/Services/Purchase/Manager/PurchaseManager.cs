using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.Purchase.CompletionInitiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Continuation;
using NasladdinPlace.Core.Services.Purchase.Initiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.Manager
{
    public class PurchaseManager : IPurchaseManager
    {
        public event EventHandler<PurchaseCompletionResult> PurchaseCompleted
        {
            add => CompletionManager.PurchaseCompleted += value;
            remove => CompletionManager.PurchaseCompleted -= value;
        }
        
        public IPurchaseInitiationManager InitiationManager { get; }
        public IPurchaseCompletionInitiationManager CompletionInitiationManager { get; }
        public IPurchaseContinuationManager ContinuationManager { get; }
        public IPurchaseCompletionManager CompletionManager { get; }
        
        public PurchaseManager(
            IPurchaseInitiationManager initiationManager,
            IPurchaseContinuationManager continuationManager,
            IPurchaseCompletionInitiationManager completionInitiationManager,
            IPurchaseCompletionManager completionManager)
        {
            if (initiationManager == null)
                throw new ArgumentNullException(nameof(initiationManager));
            if (continuationManager == null)
                throw new ArgumentNullException(nameof(continuationManager));
            if (completionInitiationManager == null)
                throw new ArgumentNullException(nameof(completionInitiationManager));
            if (completionManager == null)
                throw new ArgumentNullException(nameof(completionManager));
            
            InitiationManager = initiationManager;
            ContinuationManager = continuationManager;
            CompletionInitiationManager = completionInitiationManager;
            CompletionManager = completionManager;
        }

        public Task<PurchaseInitiationResult> InitiateAsync(PurchaseInitiationParams initiationParams)
        {
            if (initiationParams == null)
                throw new ArgumentNullException(nameof(initiationParams));

            return InitiationManager.InitiatePurchaseAsync(initiationParams);
        }

        public Task<Result> ContinuePurchaseAsync(PurchaseOperationParams continuationParams)
        {
            if (continuationParams == null)
                throw new ArgumentNullException(nameof(continuationParams));

            return ContinuationManager.ContinuePurchaseAsync(continuationParams);
        }

        public Task<Result> InitiateCompletionAsync(PurchaseOperationParams initiationParams)
        {
            if (initiationParams == null)
                throw new ArgumentNullException(nameof(initiationParams));

            return CompletionInitiationManager.InitiatePurchaseCompletionAsync(initiationParams);
        }

        public Task<PurchaseCompletionResult> CompleteLastUnpaidAsync(PurchaseOperationParams completionParams)
        {
            if (completionParams == null)
                throw new ArgumentNullException(nameof(completionParams));

            return CompletionManager.CompleteLastPurchaseOfUserAsync(completionParams.UserId);
        }

        public Task<PurchaseCompletionResult> CompleteLastUnpaidAsync(IUnitOfWork unitOfWork, PurchaseOperationParams completionParams)
        {
            return CompletionManager.CompleteLastPurchaseOfUserAsync(unitOfWork, completionParams.UserId);
        }

        public Task<PurchaseCompletionResult> CompleteUnpaidPurchaseAsync(PurchaseOperationParams completionParams,
            int posOperationId, int? paymentCardId = null)
        {
            if (completionParams == null)
                throw new ArgumentNullException(nameof(completionParams));

            return CompletionManager.CompleteUnpaidPurchaseOfUserAsync(completionParams.UserId, posOperationId,
                paymentCardId);
        }

        public Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllUnpaidAsync(PurchaseOperationParams completionParams)
        {
            return CompletionManager.CompleteAllPurchasesOfUserAsync(completionParams.UserId);
        }
    }
}