using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface ILabeledGoodTrackingRecordRepository : IRepository<LabeledGoodTrackingRecord>
    {
        IQueryable<LabeledGoodTrackingRecord> GetAllIncludingPosAndLabeledGood();
        Task<List<LabeledGoodTrackingRecord>> GetAllInDateRangeAsync(DateTimeRange dateTimeRange);
    }
}