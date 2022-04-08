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
    [Obsolete("Will be removed in the future releases")]
    public class FiscalizationInfosRepository : Repository<FiscalizationInfo>, IFiscalizationInfosRepository
    {
        public FiscalizationInfosRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public Task<List<FiscalizationInfo>> GetAllByPosOperationIdAsync(int posOperationId)
        {
            return GetAll()
                .Where(f => f.PosOperationId == posOperationId)
                .OrderByDescending(f => f.DateTimeRequest)
                .ToListAsync();
        }

        public List<FiscalizationInfo> GetAllPendingMoreThan(TimeSpan waitingFiscalizationTime)
        {
            var subtractUtcDateTime = DateTime.UtcNow.Subtract(waitingFiscalizationTime);

            return GetAll()
                .Where(f =>
                    (f.State == FiscalizationState.Pending || f.State == FiscalizationState.PendingError) &&
                    f.DateTimeRequest < subtractUtcDateTime)
                .ToList();
        }

        public Task<FiscalizationInfo> GetByIdIncludingPosOperationAndFiscalizationCheckItemsAsync(int id)
        {
            return GetAll()
                .Include(fi => fi.PosOperation)
                    .ThenInclude(p => p.User)
                .Include(fi => fi.PosOperation)
                    .ThenInclude(p => p.Pos)
                .Include(fi => fi.PosOperation)
                    .ThenInclude(p => p.PosOperationTransactions)
                        .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                            .ThenInclude(poc => poc.CheckItem)
                                .ThenInclude(cki => cki.Good)
                .Include(fi => fi.FiscalizationCheckItems)
                    .ThenInclude(ci => ci.CheckItem)
                        .ThenInclude(g => g.Good)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task<FiscalizationInfo> GetByActualPosOperationIdAsync(int posOperationId)
        {
            return GetAll().Where(f => f.PosOperationId == posOperationId)
                .OrderByDescending(p => p.DateTimeRequest)
                .FirstOrDefaultAsync();
        }

        public Task<FiscalizationInfo> GetIncludingFiscalizationCheckItemsByInitialPosOperationIdAsync(int posOperationId)
        {
            return GetAll()
                .Include(fi => fi.FiscalizationCheckItems)
                    .ThenInclude(ci => ci.CheckItem)
                        .ThenInclude(g => g.Good)
                .Where(f => f.PosOperationId == posOperationId)
                .OrderBy(p => p.DateTimeRequest)
                .FirstOrDefaultAsync();
        }

        public Task<FiscalizationInfo> GetByRequestIdAsync(Guid requestId)
        {
            return GetAll().FirstOrDefaultAsync(f => f.RequestId == requestId);
        }
    }
}
