using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;

namespace NasladdinPlace.Core.Services.Purchase.Completion
{
    public class PurchaseCompletionManager : IPurchaseCompletionManager
    {
        private readonly ICheckPaymentService _checkPaymentService;
        private readonly IUserLatestOperationCheckMaker _userLatestOperationCheckMaker;
        private readonly IUsersUnpaidOperationsChecksMaker _usersUnpaidOperationsChecksMaker;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOperationsManager _operationsManager;
        private readonly IOperationTransactionManager _operationTransacitonsManager;
        private readonly ILogger _logger;
        private readonly ISimpleCheckMaker _simpleCheckMaker;
        private readonly IPaymentInfoCreator _paymentInfoCreator;
        private readonly ICheckOnlineManager _checkOnlineManager;
        private readonly IPaymentCardsService _paymentCardsService;

        public PurchaseCompletionManager(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _checkPaymentService = serviceProvider.GetRequiredService<ICheckPaymentService>();
            _userLatestOperationCheckMaker = serviceProvider.GetRequiredService<IUserLatestOperationCheckMaker>();
            _usersUnpaidOperationsChecksMaker = serviceProvider.GetRequiredService<IUsersUnpaidOperationsChecksMaker>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _operationsManager = serviceProvider.GetRequiredService<IOperationsManager>();
            _operationTransacitonsManager = serviceProvider.GetRequiredService<IOperationTransactionManager>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _simpleCheckMaker = serviceProvider.GetRequiredService<ISimpleCheckMaker>();
            _paymentInfoCreator = serviceProvider.GetRequiredService<IPaymentInfoCreator>();
            _checkOnlineManager = serviceProvider.GetRequiredService<ICheckOnlineManager>();
            _paymentCardsService = serviceProvider.GetRequiredService<IPaymentCardsService>();
        }

        public event EventHandler<PurchaseCompletionResult> PurchaseCompleted;

        public async Task<PurchaseCompletionResult> CompleteUnpaidPurchaseOfUserAsync(int userId, int posOperationId, int? paymentCardId = null)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await CompleteUnpaidPurchaseOfUserAsync(unitOfWork, userId, posOperationId, paymentCardId);
            }
        }

        public async Task<PurchaseCompletionResult> CompleteUnpaidPurchaseOfUserAsync(IUnitOfWork unitOfWork,
            int userId, int posOperationId,
            int? paymentCardId = null)
        {
            var checkMakerResult =
                await _userLatestOperationCheckMaker.MakeForUserByUnpaidOperationAsync(unitOfWork, userId,
                    posOperationId);

            return await CompletePurchaseOfUserAsync(unitOfWork, checkMakerResult, userId, paymentCardId);
        }

        public async Task<PurchaseCompletionResult> CompleteLastPurchaseOfUserAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await CompleteLastPurchaseOfUserAsync(unitOfWork, userId);
            }
        }

        public async Task<PurchaseCompletionResult> CompleteLastPurchaseOfUserAsync(IUnitOfWork unitOfWork, int userId)
        {
            var checkMakerResult = await _userLatestOperationCheckMaker.MakeForUserIfOperationUnpaidAsync(unitOfWork, userId);

            return await CompletePurchaseOfUserAsync(unitOfWork, checkMakerResult, userId);
        }

        public async Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllPurchasesOfUserAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await CompleteAllPurchasesOfUserAsync(unitOfWork, userId);
            }
        }

        public async Task<IReadOnlyCollection<PurchaseCompletionResult>> CompleteAllPurchasesOfUserAsync(IUnitOfWork unitOfWork, int userId)
        {
            var posOperationsWithChecks = await _usersUnpaidOperationsChecksMaker.MakeForUserAsync(unitOfWork, userId);

            var user = unitOfWork.Users.GetById(userId);

            var purchaseCompletionResults = new List<PurchaseCompletionResult>();

            if (!posOperationsWithChecks.Any())
            {
                purchaseCompletionResults.Add(PurchaseCompletionResult.UnpaidPurchaseNotFound(user));
                return purchaseCompletionResults;
            }

            foreach (var operationWithCheck in posOperationsWithChecks)
            {
                var purchaseCompletionResult = await ProcessPurchaseForUserAsync(unitOfWork, operationWithCheck.CheckPosOperation, operationWithCheck.Check, user);
                purchaseCompletionResults.Add(purchaseCompletionResult);
                if (!purchaseCompletionResult.IsSuccess)
                    break;
            }

            return purchaseCompletionResults;
        }

        public async Task<PurchaseCompletionResult> CompletePurchaseByPosOperationIdAsync(
            int posOperationId, 
            int userId, 
            PosOperationTransactionType operationTransactionType = PosOperationTransactionType.RegularPurchase)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await CompletePurchaseByPosOperationIdAsync(unitOfWork, posOperationId, userId, operationTransactionType);
            }
        }

        public async Task<PurchaseCompletionResult> CompletePurchaseByPosOperationIdAsync(
            IUnitOfWork unitOfWork,
            int posOperationId,
            int userId,
            PosOperationTransactionType operationTransactionType = PosOperationTransactionType.RegularPurchase)
        {
            var user = unitOfWork.Users.GetById(userId);

            var posOperation = await unitOfWork.PosOperations.GetCompletedUnpaidHavingUnpaidCheckItemsAndPosOperationTransactionsByIdAsync(posOperationId);

            if (posOperation == null)
                return PurchaseCompletionResult.UnpaidPurchaseNotFound(user);

            var check = _simpleCheckMaker.MakeCheck(posOperation);

            return await ProcessPurchaseForUserAsync(unitOfWork, posOperation, check, user, operationTransactionType);
        }

        public async Task CompletePurchasesOfUsersAsync(IEnumerable<int> userIds)
        {
            foreach (var userId in userIds.ToImmutableHashSet())
            {
                await CompleteAllPurchasesOfUserAsync(userId);
            }
        }

        private async Task<PurchaseCompletionResult> CompletePurchaseOfUserAsync(IUnitOfWork unitOfWork,
            UserLatestOperationCheckMakerResult checkMakerResult, int userId,
            int? paymentCardId = null)
        {
            var user = unitOfWork.Users.GetById(userId);

            if (checkMakerResult.Status == UserOperationCheckMakerStatus.PosOperationNotFound)
                return PurchaseCompletionResult.UnpaidPurchaseNotFound(user);

            if (checkMakerResult.Status != UserOperationCheckMakerStatus.Success)
                return PurchaseCompletionResult.UnknownError(
                    user,
                    $"Unable to create check for user {userId} because {checkMakerResult.Status}."
                );

            var check = checkMakerResult.Check;
            var latestUnpaidUserPosOperation = checkMakerResult.CheckPosOperation;

            return await ProcessPurchaseForUserAsync(unitOfWork, latestUnpaidUserPosOperation, check, user,
                PosOperationTransactionType.RegularPurchase,
                paymentCardId);
        }

        private async Task<PurchaseCompletionResult> ProcessPurchaseForUserAsync(
            IUnitOfWork unitOfWork,
            PosOperation posOperation,
            SimpleCheck check,
            ApplicationUser user,
            PosOperationTransactionType operationTransactionType = PosOperationTransactionType.RegularPurchase,
            int? paymentCardId = null)
        {
            if (!posOperation.GetNewPaymentSystemFlag())
                return await CompletePurchaseForUserAsync(unitOfWork, posOperation, check, user, operationTransactionType, paymentCardId);

            var posOperationTransactions = posOperation.GetUnpaidTransactions();
            var purchaseCompletionResult = PurchaseCompletionResult.UnpaidPurchaseNotFound(user);
            foreach (var unpaidPosOperationTransaciton in posOperationTransactions)
            {
                purchaseCompletionResult = unpaidPosOperationTransaciton.Type == PosOperationTransactionType.RegularPurchase
                    ? await CompletePurchaseForUserAsync(unitOfWork, posOperation, check, user,
                        unpaidPosOperationTransaciton.Type, paymentCardId)
                    : await CompletePurchaseByTransactionForUserAsync(unitOfWork, posOperation, check, unpaidPosOperationTransaciton,
                        user, paymentCardId);

                if (!purchaseCompletionResult.IsSuccess)
                    return purchaseCompletionResult;
            }

            return purchaseCompletionResult;
        }

        private async Task<PurchaseCompletionResult> CompletePurchaseForUserAsync(
            IUnitOfWork unitOfWork,
            PosOperation unpaidUserPosOperation,
            SimpleCheck check,
            ApplicationUser user,
            PosOperationTransactionType transactionType = PosOperationTransactionType.RegularPurchase,
            int? paymentCardId = null)
        {
            if (unpaidUserPosOperation.Status == PosOperationStatus.PendingPayment)
                return PurchaseCompletionResult.AlreadyPendingPayment(user, unpaidUserPosOperation, check);

            var operationTransaction = GetPosOperationTransaction(unitOfWork, unpaidUserPosOperation, transactionType);

            var setPendingPaymentResult =
                await _operationsManager.MarkPosOperationAsPendingPaymentAsync(unitOfWork,
                    unpaidUserPosOperation);
            if (!setPendingPaymentResult.Succeeded)
                return PurchaseCompletionResult.UnknownError(user, setPendingPaymentResult.Error);

            var getPaymentCardResult = await _paymentCardsService.GetPaymentCardForPaymentAsync(user.Id, paymentCardId);

            var paymentInfo = _paymentInfoCreator.Create(unpaidUserPosOperation.GetNewPaymentSystemFlag(), operationTransaction, check, getPaymentCardResult.Value);

            var checkPaymentResult = await _checkPaymentService.PayForCheckAsync(unitOfWork, user.Id, paymentInfo);

            PurchaseCompletionResult purchaseCompletionResult;

            var paymentStatus = checkPaymentResult.PaymentStatus;

            if (paymentStatus == CheckPaymentStatus.Error)
            {
                purchaseCompletionResult = PurchaseCompletionResult.PaymentError(
                    user, check, checkPaymentResult.Error
                );

                AddErrorBankTransactionInfosIfNeeded(unpaidUserPosOperation, operationTransaction, checkPaymentResult);

                await _operationsManager.MarkPosOperationAsCompletedAsync(
                    unitOfWork,
                    unpaidUserPosOperation
                );

                await TrySetTransactionStatus(
                    unitOfWork,
                    operationTransaction,
                    transaction =>
                    {
                        transaction.MarkAsInProcess();
                        transaction.MarkAsUnpaid();
                    });

                NotifyPurchaseCompleted(purchaseCompletionResult);
                return purchaseCompletionResult;
            }

            var operationPaymentInfo = OperationPaymentInfo.FromCheckPaymentResult(user.Id, checkPaymentResult);
            var markingAsPaidResult =
                await _operationsManager.MarkPosOperationAsPaidAsync(unitOfWork, unpaidUserPosOperation,
                    operationPaymentInfo);

            if (!markingAsPaidResult.Succeeded)
                return PurchaseCompletionResult.UnknownError(user, "Operation does not exist.");

            await TrySetTransactionStatus(
                unitOfWork,
                operationTransaction,
                transaction =>
                {
                    transaction.MarkAsInProcess();
                    transaction.MarkAsPaid(operationPaymentInfo);
                });

            var operation = markingAsPaidResult.Value;

            var paidCheckItemsWithDiscountSum = operation.FindCheckItemsWithStatuses(CheckItemStatus.Paid)
                .Sum(x => x.PriceWithDiscount);

            if (operation.GetNewPaymentSystemFlag())
            {
                if (operationTransaction.FiscalizationAmount > 0)
                    await _checkOnlineManager.MakeFiscalizationAsync(unitOfWork, operation, operationTransaction);
            }
            else
            {
                if (paidCheckItemsWithDiscountSum > operation.BonusAmount)
                    await _checkOnlineManager.MakeFiscalizationAsync(unitOfWork, operation, operationTransaction);
            }
            
            purchaseCompletionResult = PurchaseCompletionResult.Success(user, operation, check);

            NotifyPurchaseCompleted(purchaseCompletionResult);

            return purchaseCompletionResult;
        }

        private async Task<PurchaseCompletionResult> CompletePurchaseByTransactionForUserAsync(
            IUnitOfWork unitOfWork,
            PosOperation posOperation,
            SimpleCheck check,
PosOperationTransaction operationTransaction,
            ApplicationUser user,
            int? paymentCardId)
        {
            var getPaymentCardResult = await _paymentCardsService.GetPaymentCardForPaymentAsync(user.Id, paymentCardId);

            var paymentInfo = _paymentInfoCreator.Create(true, operationTransaction, SimpleCheck.Empty, getPaymentCardResult.Value);

            var checkPaymentResult = await _checkPaymentService.PayForCheckAsync(unitOfWork, user.Id, paymentInfo);

            PurchaseCompletionResult purchaseCompletionResult;

            var paymentStatus = checkPaymentResult.PaymentStatus;

            if (paymentStatus == CheckPaymentStatus.Error)
            {
                purchaseCompletionResult = PurchaseCompletionResult.PaymentError(
                    user, check, checkPaymentResult.Error
                );

                AddErrorBankTransactionInfosIfNeeded(posOperation, operationTransaction, checkPaymentResult);

                await unitOfWork.CompleteAsync();

                await TrySetTransactionStatus(
                    unitOfWork,
                    operationTransaction,
                    transaction =>
                    {
                        transaction.MarkAsInProcess();
                        transaction.MarkAsUnpaid();
                    });

                NotifyPurchaseCompleted(purchaseCompletionResult);
                return purchaseCompletionResult;
            }

            var operationPaymentInfo = OperationPaymentInfo.FromCheckPaymentResult(user.Id, checkPaymentResult);

            var bankTransactionInfo = BankTransactionInfo.ForPayment(
                paymentCardId: checkPaymentResult.PaymentCardId.Value,
                bankTransactionId: checkPaymentResult.TransactionId.Value,
                amount: checkPaymentResult.PaymentInfo.CheckCostInMoney
            );
            posOperation.BankTransactionInfos.Add(bankTransactionInfo);

            await TrySetTransactionStatus(
                unitOfWork,
                operationTransaction,
                transaction =>
                {
                    transaction.MarkAsInProcess();
                    transaction.MarkAsPaid(operationPaymentInfo);
                });

            try
            {
                if (posOperation.Status == PosOperationStatus.Completed)
                {
                    posOperation.MarkAsPendingPayment();
                    posOperation.MarkAsPaid();
                }

                var unpaidCheckItemsInTransaction = operationTransaction.PosOperationTransactionCheckItems
                    .Where(potcki => potcki.CheckItem.Status == CheckItemStatus.Unpaid)
                    .Select(potcki => potcki.CheckItem)
                    .ToList();

                unpaidCheckItemsInTransaction.ForEach(cki => cki.MarkAsPaid());

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying set status to posOperation {posOperation.Id} and check itmes. Error: {ex}");
            }

            var fiscalizationAmount = operationTransaction.FiscalizationAmount;

            if (fiscalizationAmount > 0)
                await _checkOnlineManager.MakeFiscalizationAsync(unitOfWork, posOperation, operationTransaction);

            purchaseCompletionResult = PurchaseCompletionResult.Success(user, posOperation, check);

            NotifyPurchaseCompleted(purchaseCompletionResult);

            return purchaseCompletionResult;
        }

        private static void AddErrorBankTransactionInfosIfNeeded(PosOperation posOperation,
            PosOperationTransaction operationTransaction, CheckPaymentResult checkPaymentResult)
        {
            if (checkPaymentResult.HasBankRequisites())
            {
                var bankTransactionInfo = BankTransactionInfo.ForError(
                    paymentCardId: checkPaymentResult.PaymentCardId.Value,
                    bankTransactionId: checkPaymentResult.TransactionId.Value,
                    amount: checkPaymentResult.PaymentInfo.CheckCostInMoney,
                    comment: checkPaymentResult.BankError
                );
                posOperation.BankTransactionInfos.Add(bankTransactionInfo);

                if (operationTransaction != null)
                {
                    var bankTransactionInfosVersionTwo = BankTransactionInfoVersionTwo.ForError(
                        paymentCardId: checkPaymentResult.PaymentCardId.Value,
                        bankTransactionId: checkPaymentResult.TransactionId.Value,
                        amount: checkPaymentResult.PaymentInfo.CheckCostInMoney,
                        comment: checkPaymentResult.BankError);
                    operationTransaction.BankTransactionInfos.Add(bankTransactionInfosVersionTwo);
                }
            }
        }

        private PosOperationTransaction GetPosOperationTransaction(
            IUnitOfWork unitOfWork,
            PosOperation unpaidUserPosOperation,
            PosOperationTransactionType transactionType)
        {
            PosOperationTransaction operationTransaction = null;

            var getOperationTransactionResult =
                _operationTransacitonsManager.GetOperationTransaction(unitOfWork, unpaidUserPosOperation, transactionType);

            if (getOperationTransactionResult.Succeeded)
                operationTransaction = getOperationTransactionResult.Value;
            else
                _logger.LogError(getOperationTransactionResult.Error);

            return operationTransaction;
        }

        private void NotifyPurchaseCompleted(PurchaseCompletionResult purchaseCompletionResult)
        {
            try
            {
                PurchaseCompleted?.Invoke(this, purchaseCompletionResult);
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        private async Task TrySetTransactionStatus(
            IUnitOfWork unitOfWork,
            PosOperationTransaction operationTransaction,
            Action<PosOperationTransaction> setStatusAction)
        {
            if (operationTransaction != null)
            {
                try
                {
                    setStatusAction(operationTransaction);

                    await unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error was occurred while trying mark operationTransaction as Paid. Error - {ex}");
                }

            }
        }
    }
}