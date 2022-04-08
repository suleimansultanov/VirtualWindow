using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class FiscalizationInfoVersionTwoRepository : Repository<FiscalizationInfoVersionTwo>, IFiscalizationInfoVersionTwoRepository
    {

        public FiscalizationInfoVersionTwoRepository(ApplicationDbContext context): base(context)
        {
        }

        public Task<List<FiscalizationInfoVersionTwo>> GetAllByPosOperationIdAsync(int posOperationTransactionId)
        {
            return GetAll()
                .Where(f => f.PosOperationTransactionId == posOperationTransactionId)
                .OrderByDescending(f => f.RequestDateTime)
                .ToListAsync();
        }

        public Task<FiscalizationInfoVersionTwo> GetByIdIncludingPosOperationTransactionAndPosOperationTransactionCheckItemsAsync(int id)
        {
            return GetAll()
                .Include(fi => fi.PosOperationTransaction)
                    .ThenInclude(pot => pot.PosOperation)
                        .ThenInclude(p => p.User)
                .Include(fi => fi.PosOperationTransaction)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                        .ThenInclude(ci => ci.CheckItem)
                            .ThenInclude(g => g.Good)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task<FiscalizationInfoVersionTwo> GetByActualPosOperationIdAsync(int posOperationTransactionId)
        {
            return GetAll()
                .Where(f => f.PosOperationTransactionId == posOperationTransactionId)
                .OrderByDescending(p => p.RequestDateTime)
                .FirstOrDefaultAsync();
        }

        public Task<FiscalizationInfoVersionTwo> GetIncludingPosOperationTransactionCheckItemsByInitialPosOperationTransactionIdAsync(
            int posOperationTransactionId)
        {
            return GetAll()
                .Include(fi => fi.PosOperationTransaction)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                        .ThenInclude(ci => ci.CheckItem)
                            .ThenInclude(g => g.Good)
                .Where(f => f.PosOperationTransactionId == posOperationTransactionId)
                .OrderBy(p => p.RequestDateTime)
                .FirstOrDefaultAsync();
        }

        public Task<FiscalizationInfoVersionTwo> GetLastIncludePosOperationTransactionByRequestIdAsync(string requestId)
        {
            return GetAll()
                .Include(f => f.PosOperationTransaction)
                .OrderBy(f => f.RequestDateTime)
                .FirstOrDefaultAsync(f => f.RequestId == requestId);
        }

        public List<FiscalizationInfoVersionTwo> GetAllPendingMoreThan(TimeSpan waitingFiscalizationTime)
        {
            var subtractUtcDateTime = DateTime.UtcNow.Subtract(waitingFiscalizationTime);

            return GetAll()
                .Where(f =>
                    (f.State == FiscalizationState.Error || f.State == FiscalizationState.PendingError) &&
                    f.RequestDateTime < subtractUtcDateTime)
                .ToList();
        }
    }
}
