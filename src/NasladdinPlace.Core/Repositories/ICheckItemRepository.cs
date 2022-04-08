using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface ICheckItemRepository : IRepository<CheckItem>
    {
        Task<List<CheckItem>> GetNotModifiedByAdminPaidOrUnpaidAppearedAfterPurchaseAsync(TimeSpan delaySincePurchaseCompletion);
        Task<List<CheckItem>> FindInPosWithExpirationDateInTimeRangeAsync(DateTimeRange dateTimeRange);
        Task<List<CheckItem>> GetNotModifiedByAdminUnverifiedAppearedAfterPurchaseAsync(TimeSpan delaySincePurchaseCompletion);
    }
}