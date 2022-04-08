using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts
{
    public interface IOperationTransactionManager
    {
        Task<Result> MarkOperationTransactionAsInProcessAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction);
        Task<Result> MarkOperationTransactionAsUnpaidAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction);
        Task<ValueResult<PosOperation>> MarkOperationTransactionAsPaidAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction, OperationPaymentInfo operationPaymentInfo);
        Task<Result> MarkOperationTransactionAsFiscalizedAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction);
        ValueResult<PosOperationTransaction> GetOperationTransaction(IUnitOfWork unitOfWork, PosOperation posOperation, PosOperationTransactionType transactionType);
        PosOperationTransaction CreateOperationTransaction(PosOperationTransactionCreationInfo posOperationTransactionCreationInfo);
        Task<ValueResult<PosOperationTransaction>> CreateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation);
        Task<Result> UpdateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation);
        Task<Result> AddOrUpdateOperationTransactionAsync(IUnitOfWork unitOfWork, PosOperation posOperation);
    }
}
