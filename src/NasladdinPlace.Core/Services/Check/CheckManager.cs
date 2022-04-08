using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check
{
    public class CheckManager : ICheckManager
    {
        public event EventHandler<IEnumerable<CheckItem>> CheckItemsDeletedOrRefunded;
        public event EventHandler<CheckEditingInfo> CheckItemsEditingCompleted;

        private readonly ICheckManagerPaymentHelper _checkManagerPaymentHelper;
        private readonly ICheckManagerRefundOrDeletionHelper _checkManagerRefundOrDeletionHelper;
        private readonly ICheckManagerOperationTransactionWrapper _checkManagerOperationTransactionWrapper;
        private readonly IPurchaseCompletionManager _purchaseCompletionManager;
        private readonly ICheckOnlineManager _checkOnlineManager;
        private readonly IOperationTransactionManager _operationTransactionManager;
        private readonly ICheckManagerBonusPointsHelper _checkManagerBonusPointsHelper;

        public CheckManager(
        ICheckManagerPaymentHelper checkManagerPaymentHelper,
        ICheckManagerRefundOrDeletionHelper checkManagerRefundOrDeletionHelper,
        ICheckManagerOperationTransactionWrapper checkManagerOperationTransactionWrapper,
        IPurchaseCompletionManager purchaseCompletionManager,
        ICheckOnlineManager checkOnlineManager,
        IOperationTransactionManager operationTransactionManager,
        ICheckManagerBonusPointsHelper checkManagerBonusPointsHelper)
        {
            _checkManagerPaymentHelper = checkManagerPaymentHelper;
            _checkManagerRefundOrDeletionHelper = checkManagerRefundOrDeletionHelper;
            _checkManagerOperationTransactionWrapper = checkManagerOperationTransactionWrapper;
            _purchaseCompletionManager = purchaseCompletionManager;
            _checkOnlineManager = checkOnlineManager;
            _operationTransactionManager = operationTransactionManager;
            _checkManagerBonusPointsHelper = checkManagerBonusPointsHelper;
        }

        public async Task<ICheckManagerResult> AddItemAsync(CheckItemAdditionInfo checkItemAdditionInfo)
        {
            var checkManagerResult = await _checkManagerOperationTransactionWrapper.ProcessCheckItemInfoInTransactionAsync(checkItemAdditionInfo, async (posOperation, unitOfWork) =>
            {
                var checkItem = CheckItem.NewBuilder(
                        posOperation.PosId,
                        checkItemAdditionInfo.PosOperationId,
                        checkItemAdditionInfo.GoodId,
                        checkItemAdditionInfo.LabeledGoodId,
                        checkItemAdditionInfo.CurrencyId)
                    .SetPrice(checkItemAdditionInfo.Price)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build();

                posOperation.AddCheckItem(checkItem);
                await unitOfWork.CompleteAsync();

                var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                    posOperation,
                    new List<CheckItem> { checkItem },
                    posOperation.User.TotalBonusPoints,
                    PosOperationTransactionType.Addition);

                var posOperationTransaction = _operationTransactionManager.CreateOperationTransaction(posOperationTransactionCreationInfo);

                if (posOperation.Status == PosOperationStatus.Completed)
                {
                    checkItem.MarkAsUnpaid(checkItemAdditionInfo.EditorId);
                    checkItem.MarkAsModifiedByAdmin();

                    posOperation.AddTransaction(posOperationTransaction);

                    //TODO: what if User has BonusPoints? Then checkItem.Price is incorrect for send as sms to User
                    return CheckManagerResult.NeedToCompletePurchase(
                        new CheckEditingInfo(posOperation, checkItem.Price, CheckEditingType.AdditionOrVerification));
                }

                var paymentResult = CheckManagerResult.Failure($"An error occured while trying add CheckItem {checkItem} to PosOperation");

                if (posOperation.Status == PosOperationStatus.Paid)
                {
                    paymentResult = await _checkManagerPaymentHelper.TryPayForCheckItemAsync(
                        unitOfWork,
                        posOperation,
                        checkItem.Price,
                        PosOperationTransactionType.Addition,
                        posOperationTransaction);

                    if (!paymentResult.IsSuccessful)
                        return paymentResult;

                    checkItem.MarkAsUnpaid(checkItemAdditionInfo.EditorId);
                    checkItem.MarkAsPaid(checkItemAdditionInfo.EditorId);
                    checkItem.MarkAsModifiedByAdmin();
                }

                return paymentResult;
            });

            if (checkManagerResult.IsSuccessful && checkManagerResult.ShouldPayForPurchase)
            {
                var purchaseResult = await _purchaseCompletionManager.CompletePurchaseByPosOperationIdAsync(
                    checkItemAdditionInfo.PosOperationId,
                    checkManagerResult.GetUserId(),
                    PosOperationTransactionType.Addition);

                if (!purchaseResult.IsSuccess)
                    return CheckManagerResult.Failure($"The Item hasn't been paid. It will be paid at 10AM. Error: {purchaseResult.Error.LocalizedDescription}");

                CheckItemsEditingCompleted?.Invoke(this, checkManagerResult.CheckEditingInfo);
                return checkManagerResult;
            }

            if (checkManagerResult.IsSuccessful && checkManagerResult.IsPaidViaMoney)
            {
                await MakeFiscalizationAsync(checkManagerResult.CheckEditingInfo, PosOperationTransactionType.Addition);
                CheckItemsEditingCompleted?.Invoke(this, checkManagerResult.CheckEditingInfo);
            }

            return checkManagerResult;
        }

        public async Task<ICheckManagerResult> MarkCheckItemsAsVerifiedAsync(CheckItemsEditingInfo checkItemsEditingInfo)
        {
            var checkManagerResult = await _checkManagerOperationTransactionWrapper.ProcessCheckItemInfoInTransactionAsync(checkItemsEditingInfo, async (posOperation, unitOfWork) =>
            {
                var checkItems = posOperation.FindCheckItemsWithStatusesByIds(
                        checkItemsEditingInfo.CheckItemsIds,
                        CheckItemStatus.Unverified,
                        CheckItemStatus.PaidUnverified
                    )
                    .ToList();

                if (!checkItems.Any())
                {
                    unitOfWork.RollbackTransaction();
                    return CheckManagerResult.Failure("Cannot verify for check items because it does not exist.");
                }

                var checkItemsToMarkAsPaid = checkItems.Where(cki => cki.Status == CheckItemStatus.PaidUnverified)
                                                       .Select(cki => cki)
                                                       .ToImmutableList();

                checkItemsToMarkAsPaid.ForEach(cki =>
                {
                    cki.MarkAsPaid();
                    cki.MarkAsModifiedByAdmin();
                });

                var checkItemsToVerify = checkItems.Where(cki => cki.Status == CheckItemStatus.Unverified)
                                                   .Select(cki => cki)
                                                   .ToImmutableList();

                if (!checkItemsToVerify.Any())
                    return CheckManagerResult.Success();

                var checkItemsPriceWithDiscount = checkItemsToVerify.Sum(cki => cki.PriceWithDiscount);

                if (posOperation.Status == PosOperationStatus.Completed)
                {
                    checkItemsToVerify.ForEach(cki =>
                    {
                        cki.MarkAsUnpaid(checkItemsEditingInfo.EditorId);
                        cki.MarkAsModifiedByAdmin();
                    });

                    var addOrUpdateOperationTransactionResult = await _operationTransactionManager.AddOrUpdateOperationTransactionAsync(unitOfWork, posOperation);

                    return !addOrUpdateOperationTransactionResult.Succeeded
                        ? CheckManagerResult.Failure(addOrUpdateOperationTransactionResult.Error)
                        : CheckManagerResult.NeedToCompletePurchase(new CheckEditingInfo(posOperation, checkItemsPriceWithDiscount, CheckEditingType.AdditionOrVerification));
                }

                var paymentResult = CheckManagerResult.Failure("An error occurred while making check editing");

                var availableBonusPoints = _checkManagerBonusPointsHelper.CalculateBonusPointsAmountThatCanBeWrittenOff(
                    posOperation,
                    PosOperationTransactionType.Verification,
                    checkItemsPriceWithDiscount);

                var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                    posOperation,
                    checkItemsToVerify,
                    availableBonusPoints,
                    PosOperationTransactionType.Verification);

                var posOperationTransaction = _operationTransactionManager.CreateOperationTransaction(posOperationTransactionCreationInfo);

                if (IsPaymentAllowed(posOperation, checkItemsToVerify))
                {
                    paymentResult = await _checkManagerPaymentHelper.TryPayForCheckItemAsync(
                        unitOfWork,
                        posOperation,
                        checkItemsPriceWithDiscount,
                        PosOperationTransactionType.Verification,
                        posOperationTransaction,
                        checkItemsToVerify);

                    if (!paymentResult.IsSuccessful)
                        return paymentResult;

                    checkItemsToVerify.ForEach(cki =>
                    {
                        cki.MarkAsUnpaid(checkItemsEditingInfo.EditorId);
                        cki.MarkAsPaid(checkItemsEditingInfo.EditorId);
                        cki.MarkAsModifiedByAdmin();
                    });
                }

                return paymentResult;
            });

            if (checkManagerResult.IsSuccessful && checkManagerResult.ShouldPayForPurchase)
            {
                var purchaseResult = await _purchaseCompletionManager.CompletePurchaseByPosOperationIdAsync(
                    posOperationId: checkItemsEditingInfo.PosOperationId,
                    userId: checkManagerResult.GetUserId());

                if (!purchaseResult.IsSuccess)
                    return CheckManagerResult.Failure(purchaseResult.Error.LocalizedDescription);

                CheckItemsEditingCompleted?.Invoke(this, checkManagerResult.CheckEditingInfo);
                return checkManagerResult;
            }

            if (checkManagerResult.IsSuccessful && checkManagerResult.IsPaidViaMoney)
            {
                await MakeFiscalizationAsync(checkManagerResult.CheckEditingInfo, PosOperationTransactionType.Verification);
                CheckItemsEditingCompleted?.Invoke(this, checkManagerResult.CheckEditingInfo);
            }

            return checkManagerResult;

        }

        public async Task<ICheckManagerResult> RefundOrDeleteItemsAsync(CheckItemsEditingInfo checkItemsDeletionInfo)
        {
            var checkManagerResult = await _checkManagerRefundOrDeletionHelper.MakeRefundOrDeleteAsync(checkItemsDeletionInfo,
                checkItemsToNotify =>
                {
                    if (!checkItemsToNotify.Any())
                        return;

                    CheckItemsDeletedOrRefunded?.Invoke(this, checkItemsToNotify);
                });

            if (checkManagerResult.IsPaidViaMoney)
            {
                await MakeFiscalizationAsync(checkManagerResult.CheckEditingInfo, PosOperationTransactionType.Refund);
                CheckItemsEditingCompleted?.Invoke(this, checkManagerResult.CheckEditingInfo);
            }

            return checkManagerResult;
        }

        private static bool IsPaymentAllowed(PosOperation posOperation, ImmutableList<CheckItem> checkItemsToVerify)
        {
            return posOperation.IsPaid && checkItemsToVerify.Any();
        }

        private Task MakeFiscalizationAsync(CheckEditingInfo checkEditingInfo, PosOperationTransactionType transactionType)
        {
            return transactionType == PosOperationTransactionType.Refund
                ? _checkOnlineManager.MakeIncomeRefundFiscalizationAsync(
                    checkEditingInfo.PosOperation.Id,
                    checkEditingInfo.CheckItemsToRefund,
                    checkEditingInfo.BonusPoints)
                : _checkOnlineManager.MakeFiscalizationCorrectionAsync(
                    checkEditingInfo.PosOperation.Id,
                    transactionType,
                    -checkEditingInfo.MoneyAmount);

        }
    }
}

