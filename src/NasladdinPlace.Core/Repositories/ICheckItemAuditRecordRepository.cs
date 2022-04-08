using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface ICheckItemAuditRecordRepository : IRepository<CheckItemAuditRecord>
    {
        Task<List<CheckItemAuditRecord>> GetIncludingCheckItemsAndUserByPosOperationIdOrderedByCreatedDateAsync(int posOperationId);
    }
}