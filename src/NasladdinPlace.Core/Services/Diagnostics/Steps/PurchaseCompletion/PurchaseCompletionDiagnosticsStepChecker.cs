using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Diagnostics.Constants;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PurchaseCompletion
{
    public class PurchaseCompletionDiagnosticsStepChecker : IDiagnosticsStepSuccessChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PurchaseCompletionDiagnosticsStepChecker(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            if (context.PosOperation == null)
                throw new NotSupportedException(
                    "Diagnostics context must have a pos operation."
                );

            return AssertStepExecutedSuccessfullyAuxAsync(context);
        }

        private async Task<Result> AssertStepExecutedSuccessfullyAuxAsync(DiagnosticsContext context)
        {
            Task.Delay(DiagnosticsConstants.ClosingDoorsTimeout).Wait();

            var posOperation = context.PosOperation;
            var posId = posOperation.PosId;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);

                if (posRealTimeInfo.DoorsState != DoorsState.DoorsClosed)
                    return Result.Failure($"The doors of POS {posId} must be closed.");
            }

            Task.Delay(DiagnosticsConstants.InventoryTimeout).Wait();

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                posOperation = await unitOfWork.PosOperations.GetAsync(posOperation.Id);

                if (!posOperation.DateCompleted.HasValue)
                    return Result.Failure(
                        $"The operation of POS {posId} and user {posOperation.UserId} must be completed."
                    );
            }
            
            return Result.Success();
        }
    }
}