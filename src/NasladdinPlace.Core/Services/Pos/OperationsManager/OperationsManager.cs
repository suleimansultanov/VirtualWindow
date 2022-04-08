using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Discounts.Managers;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public class OperationsManager : IOperationsManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDiscountsCheckManager _discountsCheckManager;
        private readonly IFirstPayBonusAdder _firstPayBonusAdder;
        private readonly IOperationTransactionManager _operationTransactionManager;
        private readonly ILogger _logger;

        public OperationsManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDiscountsCheckManager discountsCheckManager,
            IFirstPayBonusAdder firstPayBonusAdder,
            IOperationTransactionManager operationTransactionManager,
            ILogger logger)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (discountsCheckManager == null)
                throw new ArgumentNullException(nameof(discountsCheckManager));
            if (firstPayBonusAdder == null)
                throw new ArgumentNullException(nameof(firstPayBonusAdder));
            if (operationTransactionManager == null)
                throw new ArgumentNullException(nameof(operationTransactionManager));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _unitOfWorkFactory = unitOfWorkFactory;
            _discountsCheckManager = discountsCheckManager;
            _firstPayBonusAdder = firstPayBonusAdder;
            _operationTransactionManager = operationTransactionManager;
            _logger = logger;
        }

        public Task<ValueResult<PosOperation>> CloseLatestUserOperationAsync(IUnitOfWork unitOfWork, int userId)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return CloseOperationAsync(unitOfWork, () =>
                unitOfWork.PosOperations.GetLatestUnpaidOfUserAsync(userId)
            );
        }

        public Task<ValueResult<PosOperation>> CloseLatestPosOperationAsync(IUnitOfWork unitOfWork, int posId)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return CloseOperationAsync(unitOfWork, () =>
                unitOfWork.PosOperations.GetLatestActiveOfPosAsync(posId)
            );
        }

        public Task<ValueResult<PosOperation>> MarkPosOperationAsPaidAsync(
            IUnitOfWork unitOfWork, PosOperation posOperation, OperationPaymentInfo operationPaymentInfo)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (operationPaymentInfo == null)
                throw new ArgumentNullException(nameof(operationPaymentInfo));

            return MarkPosOperationAsPaidAuxAsync(unitOfWork, posOperation, operationPaymentInfo);
        }

        private async Task<ValueResult<PosOperation>> MarkPosOperationAsPaidAuxAsync(IUnitOfWork unitOfWork, PosOperation posOperation,
            OperationPaymentInfo operationPaymentInfo)
        {
            if (posOperation.Status != PosOperationStatus.PendingPayment)
                return ValueResult<PosOperation>.Failure(
                    "Pos operation is not in pending payment status. " +
                    $"Found status is {posOperation.Status}."
                );

            try
            {
                posOperation.MarkAsPaid(operationPaymentInfo);
                await unitOfWork.CompleteAsync();

                await _firstPayBonusAdder.CheckAndAddAvailableUserBonusPointsAsync(operationPaymentInfo.UserId);

                return ValueResult<PosOperation>.Success(posOperation);
            }
            catch (Exception ex)
            {
                return ValueResult<PosOperation>.Failure(ex.ToString());
            }
        }

        public Task<Result> MarkPosOperationAsCompletedAsync(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            return TrySetPosOperationStatus(
                unitOfWork,
                posOperation,
                operation =>
                {
                    operation.MarkAsCompleted();
                });
        }

        public Task<Result> MarkPosOperationAsPendingPaymentAsync(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            return TrySetPosOperationStatus(
                unitOfWork,
                posOperation,
                operation =>
                {
                    posOperation.MarkAsPendingPayment();
                });
        }

        private async Task<Result> TrySetPosOperationStatus(IUnitOfWork unitOfWork, PosOperation posOperation, Action<PosOperation> setPosOperationStatus)
        {
            try
            {
                setPosOperationStatus(posOperation);
                await unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    $"Exception is occured while trying set pos operation status. " +
                    $"Exception: {ex}"
                );
            }
        }

        public Task<OperationsManagerResult> TryCreateOperationAsync(OperationCreationParams creationParams)
        {
            if (creationParams == null)
                throw new ArgumentNullException(nameof(creationParams));

            return TryCreateOperationAuxAsync(creationParams);
        }

        private async Task<OperationsManagerResult> TryCreateOperationAuxAsync(
            OperationCreationParams creationParams)
        {
            var posId = creationParams.PosId;
            var mode = creationParams.PosMode;
            var userId = creationParams.UserId;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var lastActivePosOperation = await unitOfWork.PosOperations.GetLatestActiveOfPosAsync(posId);

                if (lastActivePosOperation == null)
                    return await CreateOperationAuxAsync(unitOfWork, creationParams);

                if (lastActivePosOperation.CanBeContinuedByUser(userId, mode))
                {
                    return OperationsManagerResult.Success(lastActivePosOperation);
                }

                var failureType = lastActivePosOperation.DoesBelongToUserInMode(userId, mode)
                    ? OperationsManagerFailureType.LastPosOperationPendingCompletion
                    : OperationsManagerFailureType.LastPosOperationBelongsToOtherUserOrMode;

                return OperationsManagerResult.Failure(failureType);
            }
        }

        private static async Task<OperationsManagerResult> CreateOperationAuxAsync(
            IUnitOfWork unitOfWork,
            OperationCreationParams creationParams)
        {
            var posId = creationParams.PosId;
            var mode = creationParams.PosMode;
            var userId = creationParams.UserId;
            var brand = creationParams.Brand;

            var pos = await unitOfWork.PointsOfSale.GetByIdIncludingAllowedOperationModesAsync(posId);

            if (pos == null || !pos.IsModeAllowed(mode))
                return OperationsManagerResult.Failure(OperationsManagerFailureType.PosModeNotAllowed);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(userId, posId)
                .SetBrand(brand)
                .SetMode(mode)
                .Build();

            unitOfWork.PosOperations.Add(posOperation);
            await unitOfWork.CompleteAsync();

            posOperation.Pos = pos;

            return OperationsManagerResult.Success(posOperation);
        }

        private async Task<ValueResult<PosOperation>> CloseOperationAsync(
            IUnitOfWork unitOfWork, Func<Task<PosOperation>> getPosOperation)
        {
            try
            {
                unitOfWork.BeginTransaction(IsolationLevel.RepeatableRead);

                var posOperation = await getPosOperation();

                if (posOperation == null)
                    return ValueResult<PosOperation>.Failure("Pos operation has not been found.");

                if (posOperation.IsCheckCreationForbidden)
                    return ValueResult<PosOperation>.Failure(
                        "Pos operation's check creation is not permitted. " +
                        $"Pos operation status is {posOperation.Status.ToString().ToLower()}."
                    );

                posOperation.MarkAsPendingCheckCreation();

                posOperation.AddCheckItems();

                await _discountsCheckManager.AddDiscountsAsync(posOperation, unitOfWork);

                posOperation.WriteOffBonusPoints();

                posOperation.MarkAsCompletedAndRememberDate();

                await unitOfWork.CompleteAsync();

                await TryCreateTransaction(unitOfWork, posOperation);

                unitOfWork.CommitTransaction();

                return ValueResult<PosOperation>.Success(posOperation);
            }
            catch (Exception ex)
            {
                unitOfWork.RollbackTransaction();

                return ValueResult<PosOperation>.Failure($"Unable to close pos operation because {ex}.");
            }
        }

        //TODO: return bool or Result when we will use new payment system
        private async Task TryCreateTransaction(IUnitOfWork unitOfWork, PosOperation posOperation)
        {
            if (posOperation.CheckItems.Any(cki => cki.Status == CheckItemStatus.Unpaid))
            {
                var createPosOperationTransactionResult =
                    await _operationTransactionManager.CreateOperationTransactionAsync(unitOfWork, posOperation);

                if (!createPosOperationTransactionResult.Succeeded)
                    _logger.LogError(createPosOperationTransactionResult.Error);
            }
        }
    }
}
