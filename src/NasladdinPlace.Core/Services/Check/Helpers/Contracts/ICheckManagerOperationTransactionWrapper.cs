using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers.Contracts
{
    public interface ICheckManagerOperationTransactionWrapper
    {
        Task<CheckManagerResult> ProcessCheckItemInfoInTransactionAsync(ICheckItemInfo checkItemInfo,
            Func<PosOperation, IUnitOfWork, Task<CheckManagerResult>> transactionAction);
    }
}
