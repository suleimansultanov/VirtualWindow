using NasladdinPlace.Core.Models.Fiscalization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IFiscalizationInfoVersionTwoRepository : IRepository<FiscalizationInfoVersionTwo>
    {
        Task<List<FiscalizationInfoVersionTwo>> GetAllByPosOperationIdAsync(int posOperationTransactionId);
        Task<FiscalizationInfoVersionTwo> GetByIdIncludingPosOperationTransactionAndPosOperationTransactionCheckItemsAsync(int id);
        Task<FiscalizationInfoVersionTwo> GetByActualPosOperationIdAsync(int posOperationTransactionId);
        Task<FiscalizationInfoVersionTwo> GetIncludingPosOperationTransactionCheckItemsByInitialPosOperationTransactionIdAsync(int posOperationTransactionId);
        Task<FiscalizationInfoVersionTwo> GetLastIncludePosOperationTransactionByRequestIdAsync(string requestId);
        List<FiscalizationInfoVersionTwo> GetAllPendingMoreThan(TimeSpan waitingFiscalizationTime);
    }
}
