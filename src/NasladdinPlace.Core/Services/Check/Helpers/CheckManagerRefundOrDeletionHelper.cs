using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Models;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckManagerRefundOrDeletionHelper : ICheckManagerRefundOrDeletionHelper
    {
        private readonly ICheckRefundCalculator _refundCalculator;
        private readonly CheckManagerPaymentHelper _checkManagerPaymentHelper;
        private readonly ICheckManagerOperationTransactionWrapper _checkManagerOperationTransactionWrapper;
        private readonly ICheckManagerBonusPointsHelper _bonusPointsHelper;
        private readonly IPosOperationTransactionCreationUpdatingService _transactionCreationUpdatingService;
        private readonly IOperationTransactionManager _operationTransactionManager;
        private readonly ConcurrentDictionary<int, object> _posOperationsExecutionDictionary;

        public CheckManagerRefundOrDeletionHelper(
            ICheckRefundCalculator refundCalculator,
            CheckManagerPaymentHelper checkManagerPaymentHelper,
            ICheckManagerOperationTransactionWrapper checkManagerOperationTransactionWrapper,
            ICheckManagerBonusPointsHelper bonusPointsHelper,
            IOperationTransactionManager operationTransactionManager,
            IPosOperationTransactionCreationUpdatingService transactionCreationUpdatingService)
        {
            _refundCalculator = refundCalculator;
            _checkManagerPaymentHelper = checkManagerPaymentHelper;
            _checkManagerOperationTransactionWrapper = checkManagerOperationTransactionWrapper;
            _bonusPointsHelper = bonusPointsHelper;
            _transactionCreationUpdatingService = transactionCreationUpdatingService;
            _operationTransactionManager = operationTransactionManager;
            _posOperationsExecutionDictionary = new ConcurrentDictionary<int, object>();
        }

        public async Task<CheckManagerResult> MakeRefundOrDeleteAsync(
            CheckItemsEditingInfo checkItemsDeletionInfo,
            Action<ICollection<CheckItem>> notifyCheckItemsDeletedOrRefunded)
        {
            var obj = new object();
            if (_posOperationsExecutionDictionary.GetOrAdd(checkItemsDeletionInfo.PosOperationId, obj) != obj)
                return CheckManagerResult.Failure("You can not work with an operation because it is executing in different thread.");

            ICollection<CheckItem> checkItemsToNotify = new List<CheckItem>();

            try
            {
                var checkManagerResult = await _checkManagerOperationTransactionWrapper.ProcessCheckItemInfoInTransactionAsync(checkItemsDeletionInfo,
                    async (posOperation, unitOfWork) =>
                    {
                        var checkItemsIdsSet = checkItemsDeletionInfo.CheckItemsIds.ToImmutableHashSet();

                        var checkItemsToRefund = posOperation.FindCheckItemsWithStatusesByIds(
                                checkItemsIdsSet,
                                CheckItemStatus.Paid,
                                CheckItemStatus.PaidUnverified
                            ).ToImmutableList();

                        decimal amountViaMoney = 0;
                        decimal bonusPointsOnRefund = 0;
                        var bankTransactionsVersionTwo = new List<BankTransactionInfoVersionTwo>();
                        if (checkItemsToRefund.Any())
                        {
                            var refundCalculationResult = _refundCalculator.Calculate(posOperation, checkItemsToRefund);
                            bonusPointsOnRefund = refundCalculationResult.BonusAmount;

                            var transactionsPermittedRefundAmounts = posOperation.BankTransactionInfos
                                .GroupBy(bki => bki.BankTransactionId)
                                .Select(group =>
                                {
                                    var transactionPaidTotal = group
                                        .Where(item => item.Type == BankTransactionInfoType.Payment)
                                        .Sum(item => item.Amount);
                                    var transactionRefundedTotal = group
                                        .Where(item => item.Type == BankTransactionInfoType.Refund)
                                        .Sum(item => item.Amount);
                                    var permittedRefundAmount = transactionPaidTotal - transactionRefundedTotal;

                                    return new PermittedRefundTransaction
                                    (
                                        group.Key,
                                        permittedRefundAmount
                                    );
                                })
                                .Where(tpm => tpm.PermittedRefundAmount > 0M);

                            if (refundCalculationResult.MoneyAmount > 0M)
                            {
                                foreach (var transaction in transactionsPermittedRefundAmounts)
                                {
                                    if (refundCalculationResult.MoneyAmount == 0M)
                                        break;

                                    var transactionRefundPayments = GetRefundAmount(refundCalculationResult, transaction);

                                    var refundResponse =
                                        await _checkManagerPaymentHelper.MakeRefundAsync(transaction.TransactionId, transactionRefundPayments);

                                    if (!refundResponse.IsSuccess || !refundResponse.Result.IsSuccessful)
                                    {
                                        var refundErrorTransactionInfo = BankTransactionInfo.ForRefundError(
                                            transaction.TransactionId,
                                            transactionRefundPayments,
                                            refundResponse.Result?.Error ?? string.Empty
                                        );

                                        bankTransactionsVersionTwo.Add(BankTransactionInfoVersionTwo.ForRefundError(
                                            transaction.TransactionId,
                                            transactionRefundPayments,
                                            refundResponse.Result?.Error ?? string.Empty
                                        ));

                                        posOperation.BankTransactionInfos.Add(refundErrorTransactionInfo);

                                        var checkEditingInfo = new CheckEditingInfo(posOperation, amountViaMoney, CheckEditingType.Refund);

                                        return CheckManagerResult.Failure(refundResponse.Error, checkEditingInfo);
                                    }

                                    amountViaMoney += transactionRefundPayments;

                                    refundCalculationResult.SubtractMoneyAmount(transactionRefundPayments);

                                    bankTransactionsVersionTwo.Add(BankTransactionInfoVersionTwo.ForRefund(transaction.TransactionId,
                                        transactionRefundPayments));

                                    posOperation.BankTransactionInfos.Add(BankTransactionInfo.ForRefund(transaction.TransactionId,
                                        transactionRefundPayments));

                                }
                            }

                            _bonusPointsHelper.RefundBonusPoints(posOperation, refundCalculationResult.BonusAmount);

                            ProcessCheckItemsAsAdmin(checkItemsToRefund, checkItemsDeletionInfo.EditorId,
                                (checkItem, editorId) =>
                                {
                                    checkItem.MarkAsRefunded(editorId);
                                    checkItem.MarkAsModifiedByAdmin();
                                });

                        }

                        var checkItemsToDelete = posOperation
                            .FindCheckItemsWithStatusesByIds(checkItemsIdsSet, CheckItemStatus.Unpaid, CheckItemStatus.Unverified)
                            .ToImmutableList();

                        var bonusPointsOnDeletion = 0M;
                        if (checkItemsToDelete.Any())
                            bonusPointsOnDeletion = _bonusPointsHelper.RefundBonusPointsForCheckItemsAndReturnCalculatedBonusPoints(posOperation, checkItemsToDelete);

                        var totalBonusPoints = bonusPointsOnRefund + bonusPointsOnDeletion;
                        var checkItemsToRefundOrDelete = checkItemsToRefund.Union(checkItemsToDelete);

                        ProcessCheckItemsAsAdmin(checkItemsToDelete, checkItemsDeletionInfo.EditorId,
                            (checkItem, editorId) =>
                            {
                                checkItem.MarkAsDeleted(checkItemsDeletionInfo.EditorId);
                                checkItem.MarkAsModifiedByAdmin();
                            });

                        await unitOfWork.CompleteAsync();

                        if (posOperation.Status != PosOperationStatus.Paid)
                        {
                            var updateTransactionResult = await _operationTransactionManager.UpdateOperationTransactionAsync(unitOfWork, posOperation);
                            if (!updateTransactionResult.Succeeded && posOperation.GetNewPaymentSystemFlag())
                                return CheckManagerResult.Failure(updateTransactionResult.Error);
                        }
                        else
                        {
                            await CreatePosOperationTransactionAndAddToRelatedPosOperationAsync(
                                unitOfWork,
                                totalBonusPoints,
                                amountViaMoney,
                                posOperation,
                                checkItemsToRefundOrDelete,
                                bankTransactionsVersionTwo);
                        }

                        checkItemsToNotify = checkItemsToDelete.Union(checkItemsToRefund).ToImmutableList();

                        var editingInfo = CheckEditingInfo.ForRefund(posOperation, amountViaMoney, bonusPointsOnRefund, checkItemsToRefund);

                        return CheckManagerResult.Success(editingInfo);
                    });

                notifyCheckItemsDeletedOrRefunded?.Invoke(checkItemsToNotify);

                return checkManagerResult;

            }
            finally
            {
                _posOperationsExecutionDictionary.TryRemove(checkItemsDeletionInfo.PosOperationId, out _);
            }
        }

        private async Task CreatePosOperationTransactionAndAddToRelatedPosOperationAsync(
            IUnitOfWork unitOfWork,
            decimal totalBonusPoints,
            decimal amountViaMoney,
            PosOperation posOperation,
            IEnumerable<CheckItem> checkItemsToRefundOrDelete,
            List<BankTransactionInfoVersionTwo> bankTransactionsVersionTwo)
        {
            if (totalBonusPoints == 0 && amountViaMoney == 0)
                return;

            var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                posOperation,
                new List<CheckItem>(checkItemsToRefundOrDelete),
                totalBonusPoints,
                PosOperationTransactionType.Refund);

            var posOperationTransaction = _transactionCreationUpdatingService.CreateTransaction(posOperationTransactionCreationInfo);

            posOperation.AddTransaction(posOperationTransaction);
            await unitOfWork.CompleteAsync();

            bankTransactionsVersionTwo.ForEach(bti => posOperationTransaction.AddBankTransaction(bti));

            if (amountViaMoney == 0 && totalBonusPoints > 0)
            {
                posOperationTransaction.MarkAsInProcess();
                posOperationTransaction.MarkAsPaidByBonusPoints();
            }
            else
            {
                posOperationTransaction.MarkAsInProcess();
                posOperationTransaction.MarkAsPaidUnfiscalized();
            }
        }

        private static decimal GetRefundAmount(RefundCalculationResult refundCalculationResult, PermittedRefundTransaction permittedRefundTransaction)
        {
            return refundCalculationResult.MoneyAmount > permittedRefundTransaction.PermittedRefundAmount
                ? permittedRefundTransaction.PermittedRefundAmount
                : refundCalculationResult.MoneyAmount;
        }

        private static void ProcessCheckItemsAsAdmin(IEnumerable<CheckItem> checkItems, int? editorId, Action<CheckItem, int?> checkItemAction)
        {
            foreach (var checkItem in checkItems)
            {
                checkItemAction?.Invoke(checkItem, editorId);
            }
        }
    }
}
