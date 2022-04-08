using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions
{
    public class OperationTransactionManager : IOperationTransactionManager
    {
        private readonly IFirstPayBonusAdder _firstPayBonusAdder;
        private readonly ICheckOnlineManager _checkOnlineManager;
        private readonly IPosOperationTransactionCreationUpdatingService _transactionCreationUpdatingService;
        private readonly ILogger _logger;

        public OperationTransactionManager(
            ICheckOnlineManager checkOnlineManager,
            IFirstPayBonusAdder firstPayBonusAdder,
            IPosOperationTransactionCreationUpdatingService transactionCreationUpdatingService,
            ILogger logger)
        {
            if (firstPayBonusAdder == null)
                throw new ArgumentNullException(nameof(firstPayBonusAdder));
            if (checkOnlineManager == null)
                throw new ArgumentNullException(nameof(checkOnlineManager));
            if (transactionCreationUpdatingService == null)
                throw new ArgumentNullException(nameof(transactionCreationUpdatingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _checkOnlineManager = checkOnlineManager;
            _firstPayBonusAdder = firstPayBonusAdder;
            _transactionCreationUpdatingService = transactionCreationUpdatingService;
            _logger = logger;
        }

        public Task<Result> MarkOperationTransactionAsInProcessAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction)
        {
            return TrySetStatusAsync(
                unitOfWork,
                posOperationTransaction,
                PosOperationTransactionStatus.InProcess,
                transaction =>
                {
                    transaction.MarkAsInProcess();
                    transaction.MarkPosOperationAsPendingPayment();
                });
        }

        public Task<Result> MarkOperationTransactionAsUnpaidAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction)
        {
            return TrySetStatusAsync(
                unitOfWork,
                posOperationTransaction,
                PosOperationTransactionStatus.Unpaid,
                transaction =>
                {
                    transaction.MarkAsUnpaid();
                });
        }

        public Task<ValueResult<PosOperation>> MarkOperationTransactionAsPaidAsync(
            IUnitOfWork unitOfWork,
            PosOperationTransaction posOperationTransaction,
            OperationPaymentInfo operationPaymentInfo)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posOperationTransaction == null)
                throw new ArgumentNullException(nameof(posOperationTransaction));
            if (operationPaymentInfo == null)
                throw new ArgumentNullException(nameof(operationPaymentInfo));

            return MarkOperationTransactionAsPaidAuxAsync(unitOfWork, posOperationTransaction, operationPaymentInfo);
        }

        public async Task<ValueResult<PosOperation>> MarkOperationTransactionAsPaidAuxAsync(
            IUnitOfWork unitOfWork,
            PosOperationTransaction posOperationTransaction,
            OperationPaymentInfo operationPaymentInfo)
        {
            try
            {
                posOperationTransaction.MarkAsPaid(operationPaymentInfo);
                await unitOfWork.CompleteAsync();

                //TODO: do bonus calculation in event
                await _firstPayBonusAdder.CheckAndAddAvailableUserBonusPointsAsync(operationPaymentInfo.UserId);

                //TODO: this is incorrect logic, think about correct logic
                if (posOperationTransaction.MoneyAmount > posOperationTransaction.BonusAmount)
                    await _checkOnlineManager.MakeFiscalizationAsync(unitOfWork, posOperationTransaction.PosOperation, posOperationTransaction);

                return ValueResult<PosOperation>.Success(posOperationTransaction.PosOperation);
            }
            catch (Exception ex)
            {
                return ValueResult<PosOperation>.Failure(ex.ToString());
            }
        }

        public Task<Result> MarkOperationTransactionAsFiscalizedAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction)
        {
            return TrySetStatusAsync(
                unitOfWork,
                posOperationTransaction,
                PosOperationTransactionStatus.PaidFiscalized,
                transaction =>
                {
                    transaction.SetBankTransactionRequisites();
                    //TODO: replace setting for fiscalizationDate from here
                    transaction.SetFiscalizationDate(DateTime.UtcNow);
                    transaction.MarkAsPaidFiscalized();
                });
        }

        public ValueResult<PosOperationTransaction> GetOperationTransaction(
            IUnitOfWork unitOfWork,
            PosOperation posOperation,
            PosOperationTransactionType transactionType)
        {
            //TODO: Just stuff for now. remake when we will understand what kind of transaciton will be returned
            return posOperation.GetTransaction(transactionType);
        }

        public async Task<ValueResult<PosOperationTransaction>> CreateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            try
            {
                var unpaidCheckItems = posOperation.FindCheckItemsWithStatuses(CheckItemStatus.Unpaid).ToImmutableList();
                var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                    posOperation,
                    unpaidCheckItems,
                    posOperation.BonusAmount,
                    PosOperationTransactionType.RegularPurchase);

                var posOperationTransaction = _transactionCreationUpdatingService.CreateTransaction(posOperationTransactionCreationInfo);

                unitOfWork.PosOperationTransactions.Add(posOperationTransaction);

                await unitOfWork.CompleteAsync();
                return ValueResult<PosOperationTransaction>.Success(posOperationTransaction);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Can not create PosOperationTransaction. Exception: {ex}";
                _logger.LogError(errorMessage);
                return ValueResult<PosOperationTransaction>.Failure(errorMessage);
            }
        }

        public PosOperationTransaction CreateOperationTransaction(PosOperationTransactionCreationInfo posOperationTransactionCreationInfo)
        {
            return _transactionCreationUpdatingService.CreateTransaction(posOperationTransactionCreationInfo);
        }

        public async Task<Result> UpdateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            try
            {
                var unpaidCheckItems = posOperation.FindCheckItemsWithStatuses(CheckItemStatus.Unpaid).ToImmutableList();
                var checkItemsTodelete = posOperation.FindCheckItemsWithStatuses(CheckItemStatus.Deleted).ToImmutableList();

                var getTransactionResult = posOperation.GetTransaction(PosOperationTransactionType.RegularPurchase);

                if (!getTransactionResult.Succeeded)
                {
                    _logger.LogError(getTransactionResult.Error);
                    return Result.Failure();
                }

                var posOperationTransaction = getTransactionResult.Value;

                var posOperationTransactionUpdatingInfo = new PosOperationTransactionUpdatingInfo(
                    posOperationTransaction,
                    unpaidCheckItems,
                    posOperation.BonusAmount);

                if (posOperation.Status != PosOperationStatus.Paid && checkItemsTodelete.Any())
                {
                    unitOfWork.PosOperationTransactionCheckItems.RemoveRange(posOperationTransaction.PosOperationTransactionCheckItems);
                    await unitOfWork.CompleteAsync();
                    posOperationTransaction.RemoveTransactionCheckItems();
                }

                _transactionCreationUpdatingService.UpdateTransaction(posOperationTransactionUpdatingInfo);

                await unitOfWork.CompleteAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Can not update PosOperationTransaction. Exception: {ex}";
                _logger.LogError(errorMessage);
                return Result.Failure(errorMessage);
            }
        }

        public async Task<Result> AddOrUpdateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            try
            {
                var getTransactionResult = posOperation.GetTransaction(PosOperationTransactionType.RegularPurchase);

                if (!getTransactionResult.Succeeded)
                {
                    var creationTransactionResult = await CreateOperationTransactionAsync(unitOfWork, posOperation);

                    if (creationTransactionResult.Succeeded)
                        return Result.Success();

                    _logger.LogError(creationTransactionResult.Error);
                    return Result.Failure();
                }

                var updatingTransactionResult = await UpdateOperationTransactionAsync(unitOfWork, posOperation);

                return updatingTransactionResult;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Can not create or update PosOperationTransaction. Exception: {ex}";
                _logger.LogError(errorMessage);
                return Result.Failure(errorMessage);
            }
        }

        private Task<Result> TrySetStatusAsync(
            IUnitOfWork unitOfWork,
            PosOperationTransaction posOperationTransaction,
            PosOperationTransactionStatus transactionStatus,
            Action<PosOperationTransaction> setStatusAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posOperationTransaction == null)
                throw new ArgumentNullException(nameof(posOperationTransaction));
            if (setStatusAction == null)
                throw new ArgumentNullException(nameof(setStatusAction));

            return TrySetStatusAuxAsync(unitOfWork, posOperationTransaction, transactionStatus, setStatusAction);
        }

        private async Task<Result> TrySetStatusAuxAsync(
            IUnitOfWork unitOfWork,
            PosOperationTransaction posOperationTransaction,
            PosOperationTransactionStatus transactionStatus,
            Action<PosOperationTransaction> setStatusAction)
        {
            try
            {
                setStatusAction(posOperationTransaction);
                await unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    $"Exception is occured while trying set {transactionStatus.ToString().ToLower()} status to pos operation transaction. Exception: {ex}"
                );
            }
        }
    }
}
