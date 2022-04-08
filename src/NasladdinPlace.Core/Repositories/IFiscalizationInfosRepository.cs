using NasladdinPlace.Core.Models.Fiscalization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    [Obsolete("Will be removed in the future releases")]
    public interface IFiscalizationInfosRepository : IRepository<FiscalizationInfo>
    {
        Task<List<FiscalizationInfo>> GetAllByPosOperationIdAsync(int posOperationId);
        List<FiscalizationInfo> GetAllPendingMoreThan(TimeSpan waitingFiscalizationTime);
        Task<FiscalizationInfo> GetByIdIncludingPosOperationAndFiscalizationCheckItemsAsync(int id);
        Task<FiscalizationInfo> GetByActualPosOperationIdAsync(int posOperationId);
        Task<FiscalizationInfo> GetIncludingFiscalizationCheckItemsByInitialPosOperationIdAsync(int posOperationId);
        Task<FiscalizationInfo> GetByRequestIdAsync(Guid requestId);
    }
}
