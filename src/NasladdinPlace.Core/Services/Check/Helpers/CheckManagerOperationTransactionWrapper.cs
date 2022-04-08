using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckManagerOperationTransactionWrapper : ICheckManagerOperationTransactionWrapper
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;

        public CheckManagerOperationTransactionWrapper(IUnitOfWorkFactory unitOfWorkFactory, ILogger logger)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task<CheckManagerResult> ProcessCheckItemInfoInTransactionAsync(ICheckItemInfo checkItemInfo,
            Func<PosOperation, IUnitOfWork, Task<CheckManagerResult>> transactionAction)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    unitOfWork.BeginTransaction(IsolationLevel.ReadCommitted);

                    var operation =
                        await unitOfWork.PosOperations.GetIncludingCheckItemsAsync(checkItemInfo
                            .PosOperationId);

                    if (operation == null)
                    {
                        unitOfWork.RollbackTransaction();
                        var errorMessage =
                            $"You can not work with an operation {checkItemInfo.PosOperationId} because it does not exist.";
                        _logger.LogError(errorMessage);
                        return CheckManagerResult.Failure(errorMessage);

                    }

                    var transactionActionResult = await transactionAction(operation, unitOfWork);

                    if (!transactionActionResult.IsSuccessful)
                    {
                        await unitOfWork.CompleteAsync();
                        unitOfWork.CommitTransaction();
                        _logger.LogError(transactionActionResult.Error);
                        return transactionActionResult;
                    }

                    operation.MarkAuditCompleted();

                    await unitOfWork.CompleteAsync();

                    unitOfWork.CommitTransaction();

                    return transactionActionResult;
                }
                catch (Exception ex)
                {
                    unitOfWork.RollbackTransaction();
                    var errorMessage = $"An error occurred during check manager operation. Error: {ex}";
                    _logger.LogError(errorMessage);
                    return CheckManagerResult.Failure(errorMessage);
                }
            }
        }
    }
}
