using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories.Utils;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosOperationRepository
    {
        Task<PosOperation> GetAsync(int id);
        Task<PosOperation> GetIncludingCheckItemsAsync(int id);
        Task<PosOperation> GetLatestActiveOfPosAsync(int posId);
        Task<PosOperation> GetLatestUnpaidOfUserAsync(int userId);
        Task<PosOperation> GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId);
        Task<PosOperation> GetFirstCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId);
        Task<PosOperation> GetUnpaidCheckItemOfUserByPosOperationIdAsync(int userId, int posOperationId);
        Task<List<PosOperation>> GetByUserIncludingCheckItemsOrderedByDateStartedAsync(int userId);
        Task<List<PosOperation>> GetByUserIncludingCheckItemsAndFiscalizationInfoOrderedByDateStartedAsync(int userId);
        Task<List<PosOperation>> GetAllAsync();
        Task<List<PosOperation>> GetInPurchaseModeInAuditRequestDateRangeAsync(DateTimeRange dateTimeRange);
        IEntityBatchesEnumerable<PosOperation> GetDetailedPaidHavingCheckItemsForRecentPeriod(int pageSize, TimeSpan operationsActualityPeriod);
        IQueryable<PosOperation> GetDetailedIncludingOnlyRequiredFields();
        Task<List<PosOperation>> GetDetailedByPosAndUserAsync(int posId, int userId);
        Task<List<PosOperation>> GetPaidIncludingCheckItemsWhereDatePaidInTimeRangeAsync(DateTimeRange dateTimeRange);
        Task<List<PosOperation>> GetUnpaidInPurchaseModeIncludingCheckItemsSinceDateTimeAsync(DateTime dateTime);
        Task<List<PosOperation>> GetFiscalizationErrorsInPurchaseModeIncludingPosOperationTransactionsSinceDateTimeAsync(DateTime dateTime);
        Task<List<PosOperation>> GetPaidHavingCheckItemsByUserIdAsync(int userId);
        Task<PosOperation> GetLastAdminPosOperationAsync(int posId);
        Task<PosOperation> GetUserLatestIncludingCheckItemsAsync(int userId);
        Task<List<PosOperation>> GetAllCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId);
        Task<PosOperation> GetCompletedUnpaidHavingUnpaidCheckItemsAndPosOperationTransactionsByIdAsync(int id);
        void Add(PosOperation posOperation);
        void Remove(PosOperation posOperation);
        Task<List<PosOperation>> GetUnhandledConditionalBeforeTimeAsync(DateTime purchasesSearchEndTime);
        Task<Dictionary<DateTime, List<PosOperation>>> GetNutrientsByUserAsync(int userId, DateTime workDate, byte countOfDays);
        Task<List<PosOperation>> GetCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode mode, DateTimeRange dateRange);
        Task<List<PosOperation>> GetNewBillingCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode mode,
            DateTimeRange dateRange);
    }
}
