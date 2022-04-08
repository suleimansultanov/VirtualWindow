using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.DAL.Repositories
{
    public class LabeledGoodTrackingRecordRepository : Repository<LabeledGoodTrackingRecord>, ILabeledGoodTrackingRecordRepository
    {
        public LabeledGoodTrackingRecordRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<LabeledGoodTrackingRecord> GetAllIncludingPosAndLabeledGood()
        {
            return GetAll()
                .Include(ltr => ltr.Pos)
                .Include(ltr => ltr.LabeledGood)
                .ThenInclude(lg => lg.Good);
        }

        public Task<List<LabeledGoodTrackingRecord>> GetAllInDateRangeAsync(
            DateTimeRange dateTimeRange)
        {
            return GetAll()
                .Where(t => t.Timestamp >= dateTimeRange.Start && t.Timestamp <= dateTimeRange.End)
                .ToListAsync();
        }
    }
}