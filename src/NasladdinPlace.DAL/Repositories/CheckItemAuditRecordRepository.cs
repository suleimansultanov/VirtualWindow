using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class CheckItemAuditRecordRepository : Repository<CheckItemAuditRecord>, ICheckItemAuditRecordRepository
    {
        public CheckItemAuditRecordRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<CheckItemAuditRecord>> GetIncludingCheckItemsAndUserByPosOperationIdOrderedByCreatedDateAsync(int posOperationId)
        {
            return Context.CheckItemsAuditHistory
                .Include(ar => ar.CheckItem)
                    .ThenInclude(cki => cki.Good)
                .Include(ar => ar.CheckItem)
                    .ThenInclude(cki => cki.LabeledGood)
                .Include(ar => ar.User)
                .Where(ar => ar.CheckItem.PosOperationId == posOperationId)
                .OrderByDescending(ar => ar.CreatedDate)
                .ToListAsync();
        }
    }
}