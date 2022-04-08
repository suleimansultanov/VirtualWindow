using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class CheckItemRepository : Repository<CheckItem>, ICheckItemRepository
    {
        public CheckItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<CheckItem>> GetNotModifiedByAdminPaidOrUnpaidAppearedAfterPurchaseAsync(TimeSpan delaySincePurchaseCompletion)
        {
            var checkItems = GetAll()
                .Include(ci => ci.LabeledGood)
                .Include(ci => ci.PosOperation)
                .AsQueryable();

            return checkItems.Where(ci => ci.PosOperation.DateCompleted != null && ci.PosOperation.Mode == PosMode.Purchase && !ci.IsModifiedByAdmin
                        && (ci.LabeledGood.LostDateTime.HasValue || ci.LabeledGood.FoundDateTime.HasValue)
                        && (ci.Status == CheckItemStatus.Paid || ci.Status == CheckItemStatus.Unpaid)
                        && (EF.Functions.DateDiffMinute( ci.PosOperation.DateCompleted, ci.LabeledGood.LostDateTime ) >= delaySincePurchaseCompletion.Minutes
                            || EF.Functions.DateDiffMinute( ci.PosOperation.DateCompleted, ci.LabeledGood.FoundDateTime ) >= delaySincePurchaseCompletion.Minutes) )
                .ToListAsync();
        }

        public Task<List<CheckItem>> GetNotModifiedByAdminUnverifiedAppearedAfterPurchaseAsync(TimeSpan delaySincePurchaseCompletion)
        {
            var checkItems = GetAll()
                .Include(ci => ci.LabeledGood)
                .Include(ci => ci.PosOperation)
                .AsQueryable();

            return checkItems.Where(ci => ci.PosOperation.DateCompleted != null && ci.PosOperation.Mode == PosMode.Purchase && !ci.IsModifiedByAdmin
                             && (ci.LabeledGood.LostDateTime.HasValue || ci.LabeledGood.FoundDateTime.HasValue)
                             && (ci.Status == CheckItemStatus.Unverified)
                             && (EF.Functions.DateDiffMinute( ci.PosOperation.DateCompleted, ci.LabeledGood.LostDateTime ) >= delaySincePurchaseCompletion.Minutes
                                 || EF.Functions.DateDiffMinute( ci.PosOperation.DateCompleted, ci.LabeledGood.FoundDateTime ) >= delaySincePurchaseCompletion.Minutes) )
                .ToListAsync();
        }

        public Task<List<CheckItem>> FindInPosWithExpirationDateInTimeRangeAsync(DateTimeRange dateTimeRange)
        {
            return GetAll()
                .Include(ci => ci.PosOperation)
                .Include(ci => ci.LabeledGood)
                .Where(ci => ci.PosOperation.Mode != PosMode.Purchase &&
                             (ci.PosOperation.DateCompleted <= dateTimeRange.End && ci.PosOperation.DateCompleted >= dateTimeRange.Start) &&
                             ci.LabeledGood.ExpirationDate < dateTimeRange.Start && !ci.LabeledGood.IsDisabled)
                .ToListAsync();
        }
    }
}