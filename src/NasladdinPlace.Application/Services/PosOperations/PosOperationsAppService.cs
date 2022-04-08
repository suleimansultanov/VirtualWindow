using System;
using NasladdinPlace.Application.Services.PosOperations.Contracts;
using NasladdinPlace.Application.Services.PosOperations.Helpers;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Application.Services.PosOperations
{
    public class PosOperationsAppService : IPosOperationsAppService
    {
        private readonly ILimitedFrequencyActionExecutor _limitedFrequencyActionExecutor;
        private readonly ILogger _logger;

        private readonly IPosOperationsAppServiceActionExecutionFrequencyInfoFactory
            _actionExecutionFrequencyInfoFactory;

        private readonly IPurchaseManager _purchaseManager;

        public PosOperationsAppService(
            ILimitedFrequencyActionExecutor limitedFrequencyActionExecutor,
            ILogger logger,
            IPosOperationsAppServiceActionExecutionFrequencyInfoFactory actionExecutionFrequencyInfoFactory,
            IPurchaseManager purchaseManager)
        {
            if (limitedFrequencyActionExecutor == null)
                throw new ArgumentNullException(nameof(limitedFrequencyActionExecutor));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (actionExecutionFrequencyInfoFactory == null)
                throw new ArgumentNullException(nameof(actionExecutionFrequencyInfoFactory));
            if (purchaseManager == null)
                throw new ArgumentNullException(nameof(purchaseManager));

            _limitedFrequencyActionExecutor = limitedFrequencyActionExecutor;
            _logger = logger;
            _actionExecutionFrequencyInfoFactory = actionExecutionFrequencyInfoFactory;
            _purchaseManager = purchaseManager;
        }

        public void ContinuePurchaseAsync(int userId)
        {
            var purchaseContinuationFrequencyInfo = _actionExecutionFrequencyInfoFactory.CreateForUser(
                userId,
                PosOperationsAppServiceAction.PurchaseContinuation
            );

            async void PurchaseContinuationAction()
            {
                var purchaseContinuationResult =
                    await _purchaseManager.ContinuePurchaseAsync(new PurchaseOperationParams(userId));

                if (purchaseContinuationResult.Succeeded)
                {
                    _logger.LogInfo($"User {userId} has been successfully continued purchase.");
                }
                else
                {
                    _logger.LogError(
                        $"User was unable to continue purchase because: {purchaseContinuationResult.Error}"
                    );
                }
            }

            _limitedFrequencyActionExecutor.TryExecuteAsync(
                PurchaseContinuationAction,
                purchaseContinuationFrequencyInfo
            );
        }

        public void InitiatePurchaseCompletionAsync(int userId)
        {
            var purchaseCompletionInitiationFrequencyInfo = _actionExecutionFrequencyInfoFactory.CreateForUser(
                userId,
                PosOperationsAppServiceAction.PurchaseCompletionInitiation
            );

            async void PurchaseCompletionInitiationAction()
            {
                var completionInitiationResult =
                    await _purchaseManager.InitiateCompletionAsync(new PurchaseOperationParams(userId));

                if (completionInitiationResult.Succeeded)
                {
                    _logger.LogInfo($"User {userId} has been successfully initiated purchase completion.");
                }
                else
                {
                    _logger.LogError($"User {userId} was unable to initiate purchase completion because: " +
                                     $"{completionInitiationResult.Error}.");
                }
            }

            _limitedFrequencyActionExecutor.TryExecuteAsync(
                PurchaseCompletionInitiationAction,
                purchaseCompletionInitiationFrequencyInfo
            );
        }
    }
}