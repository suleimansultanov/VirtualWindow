using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Purchase.Completion;
using NasladdinPlace.Core.Services.Purchase.CompletionInitiation;
using NasladdinPlace.Core.Services.Purchase.Continuation;
using NasladdinPlace.Core.Services.Purchase.Initiation;
using NasladdinPlace.Core.Services.Purchase.Manager;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Users.Test;
using System;

namespace NasladdinPlace.Core.Services.Purchase.Factory
{
    public class PurchaseManagerFactory : IPurchaseManagerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PurchaseManagerFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IPurchaseManager Create()
        {
            var testUserChecker = GetRequiredService<ITestUserInfoProvider>();
            var ongoingPurchaseActivityManager = GetRequiredService<IOngoingPurchaseActivityManager>();
            var posInteractor = GetRequiredService<IPosInteractor>();

            var purchaseInitiationManager = new PurchaseInitiationManager(_serviceProvider);
            var continuationManager = new PurchaseContinuationManager(
                posInteractor,
                ongoingPurchaseActivityManager
            );
            var purchaseCompletionInitiationManager = new PurchaseCompletionInitiationManager(
                testUserChecker,
                posInteractor,
                ongoingPurchaseActivityManager
            );
            var purchaseCompletionManager = new PurchaseCompletionManager(_serviceProvider);

            return new PurchaseManager(
                purchaseInitiationManager,
                continuationManager,
                purchaseCompletionInitiationManager,
                purchaseCompletionManager
            );
        }

        private T GetRequiredService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}