using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts
{
    public interface IPosOperationTransactionCreationUpdatingService
    {
        PosOperationTransaction CreateTransaction(PosOperationTransactionCreationInfo transactionCreationInfo);
        PosOperationTransaction UpdateTransaction(PosOperationTransactionUpdatingInfo operationTransactionUpdatingInfo);
    }
}
