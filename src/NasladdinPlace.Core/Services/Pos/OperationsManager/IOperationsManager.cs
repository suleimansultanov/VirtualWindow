using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public interface IOperationsManager
    {
        Task<ValueResult<PosOperation>> CloseLatestUserOperationAsync(IUnitOfWork unitOfWork, int userId);
        Task<ValueResult<PosOperation>> CloseLatestPosOperationAsync(IUnitOfWork unitOfWork, int posId);
        Task<ValueResult<PosOperation>> MarkPosOperationAsPaidAsync(
            IUnitOfWork unitOfWork, PosOperation posOperation, OperationPaymentInfo operationPaymentInfo);
        Task<Result> MarkPosOperationAsCompletedAsync(IUnitOfWork unitOfWork, PosOperation posOperation);
        Task<Result> MarkPosOperationAsPendingPaymentAsync(IUnitOfWork unitOfWork, PosOperation posOperation);
        Task<OperationsManagerResult> TryCreateOperationAsync(
            OperationCreationParams creationParams
        );
    }
}
