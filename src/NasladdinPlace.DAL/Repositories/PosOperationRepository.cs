using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Repositories.Utils;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.Utils.EntityBatchesEnumeration;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosOperationRepository : IPosOperationRepository
    {
        private readonly IApplicationDbContext _context;

        public PosOperationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<PosOperation> GetAsync(int id)
        {
            return _context.PosOperations
                .Include(po => po.BankTransactionInfos)
                .Include(po => po.Pos)
                .SingleOrDefaultAsync(po => po.Id == id);
        }

        public Task<PosOperation> GetIncludingCheckItemsAsync(int id)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.LabeledGood)
                    .ThenInclude(lg => lg.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Good)
                    .ThenInclude(g => g.GoodCategory)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Currency)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Pos)
                .Include(po => po.Pos)
                .Include(po => po.User).ThenInclude(u => u.BonusPoints)
                .Include(po => po.User).ThenInclude(u => u.ActivePaymentCard)
                .Include(po => po.LabeledGoods).ThenInclude(lg => lg.Currency)
                .Include(po => po.BankTransactionInfos)
                .Include(po => po.FiscalizationInfos)
                    .ThenInclude(fi => fi.FiscalizationCheckItems)
                .Include(po => po.FiscalizationInfos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.AuditRecords)
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.BankTransactionInfos)
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.FiscalizationInfos)
                .SingleOrDefaultAsync(po => po.Id == id);
        }

        public Task<List<PosOperation>> GetByUserIncludingCheckItemsOrderedByDateStartedAsync(int userId)
        {
            return GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
                .Where(po => po.UserId == userId)
                .OrderByDescending(po => po.DateStarted)
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetByUserIncludingCheckItemsAndFiscalizationInfoOrderedByDateStartedAsync(int userId)
        {
            return GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
                .Include(po => po.FiscalizationInfos)
                    .ThenInclude(fi => fi.FiscalizationCheckItems)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.AuditRecords)
                .Include(po => po.BankTransactionInfos)
                .Where(po => po.UserId == userId)
                .OrderByDescending(po => po.DateStarted)
                .ToListAsync();
        }

        public Task<PosOperation> GetUserLatestIncludingCheckItemsAsync(int userId)
        {

            return GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
                .OrderByDescending(po => po.DateStarted)
                .FirstOrDefaultAsync(po => po.UserId == userId);
        }

        public Task<PosOperation> GetLatestActiveOfPosAsync(int posId)
        {
            var posOperation = GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
                                .Include(po => po.LabeledGoods)
                                .OrderByDescending(upt => upt.DateStarted)
                                .FirstOrDefault(upt => upt.PosId == posId);
            return Task.FromResult(posOperation?.DateCompleted != null
                ? null
                : posOperation
            );
        }

        public Task<PosOperation> GetLatestUnpaidOfUserAsync(int userId)
        {
            return GetLatestOfUserByPredicateAsync(po => po.UserId == userId && po.DatePaid == null);
        }

        public Task<PosOperation> GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId)
        {
            return GetCompletedUnpaidHavingUnpaidCheckItems()
                .Include(po => po.BankTransactionInfos)
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .OrderByDescending(upt => upt.DateStarted)
                .FirstOrDefaultAsync(po =>
                    po.UserId == userId &&
                    po.DateCompleted != null &&
                    po.DatePaid == null &&
                    po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid));
        }

        public Task<PosOperation> GetFirstCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId)
        {
            return GetCompletedUnpaidHavingUnpaidCheckItems()
                .Include(po => po.BankTransactionInfos)
                .Include(po => po.PosOperationTransactions)
                .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .OrderBy(upt => upt.DateStarted)
                .FirstOrDefaultAsync(po =>
                    po.UserId == userId &&
                    po.DateCompleted != null &&
                    po.DatePaid == null &&
                    po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid));
        }

        public Task<PosOperation> GetCompletedUnpaidHavingUnpaidCheckItemsAndPosOperationTransactionsByIdAsync(int id)
        {
            return GetCompletedUnpaidHavingUnpaidCheckItems()
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .FirstOrDefaultAsync(po =>
                    po.Mode == PosMode.Purchase &&
                    po.Id == id &&
                    po.DateCompleted != null &&
                    po.DatePaid == null &&
                    po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid));
        }

        public Task<List<PosOperation>> GetAllCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(int userId)
        {
            return GetCompletedUnpaidHavingUnpaidCheckItems()
                .Include(po => po.PosOperationTransactions)
                    .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .Where(po => po.UserId == userId &&
                             po.Mode == PosMode.Purchase &&
                             po.DateCompleted != null &&
                             po.DatePaid == null &&
                             po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid))
                .OrderByDescending(upt => upt.DateStarted)
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetAllAsync()
        {
            return _context.PosOperations.ToListAsync();
        }

        public IQueryable<PosOperation> GetDetailedPosOperations()
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.LabeledGood)
                    .ThenInclude(lg => lg.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Currency)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Good)
                    .ThenInclude(c => c.GoodCategory)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Pos)
                .Include(po => po.Pos)
                .Include(po => po.User)
                .Include(po => po.LabeledGoods)
                    .ThenInclude(lg => lg.Good)
                .Include(po => po.LabeledGoods)
                    .ThenInclude(lg => lg.Currency)
                .Include(po => po.FiscalizationInfos)
                .Include(po => po.PosDoorsStates)
                .Include(po => po.BankTransactionInfos)
                    .ThenInclude(bti => bti.PaymentCard)
                .OrderByDescending(po => po.DateCompleted)
                .AsQueryable();
        }

        public IEntityBatchesEnumerable<PosOperation> GetDetailedPaidHavingCheckItemsForRecentPeriod(int pageSize, TimeSpan operationsActualityPeriod)
        {
            var operationsSearchStartDate = DateTime.UtcNow.AddDays(-operationsActualityPeriod.Days);
            var posOperations = GetDetailedPosOperations()
                .AsNoTracking()
                .Where(po => po.Status == PosOperationStatus.Paid && po.CheckItems.Any() && po.DatePaid >= operationsSearchStartDate)
                .OrderByDescending(po => po.DatePaid);
            return new EntityBatchesEnumerable<PosOperation>(posOperations, pageSize);
        }

        public IQueryable<PosOperation> GetDetailedIncludingOnlyRequiredFields()
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                .Include(po => po.Pos)
                .Include(po => po.User)
                .Include(po => po.FiscalizationInfos)
                .Include(po => po.BankTransactionInfos)
                    .ThenInclude(bti => bti.PaymentCard)
                .Include(po => po.DocumentsGoodsMoving)
                .OrderByDescending(po => po.DateCompleted)
                .AsQueryable();
        }

        public Task<List<PosOperation>> GetDetailedByPosAndUserAsync(int posId, int userId)
        {
            return GetDetailedPosOperations()
                .Where(po => po.PosId == posId && po.UserId == userId)
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetInPurchaseModeInAuditRequestDateRangeAsync(DateTimeRange dateTimeRange)
        {
            return _context.PosOperations
                .Where(p => p.AuditRequestDateTime.HasValue &&
                            p.AuditRequestDateTime.Value > dateTimeRange.Start &&
                            p.AuditRequestDateTime.Value <= dateTimeRange.End &&
                            p.Mode == PosMode.Purchase)
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetPaidIncludingCheckItemsWhereDatePaidInTimeRangeAsync(DateTimeRange dateTimeRange)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                .Where(po => po.Mode == PosMode.Purchase &&
                             po.Status == PosOperationStatus.Paid && po.DatePaid != null &&
                             po.DatePaid >= dateTimeRange.Start && po.DatePaid <= dateTimeRange.End &&
                             po.CheckItems.Any(c =>
                                 c.Status == CheckItemStatus.Paid))
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetUnpaidInPurchaseModeIncludingCheckItemsSinceDateTimeAsync(
            DateTime dateTime)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                .Where(po => po.Mode == PosMode.Purchase &&
                             po.Status != PosOperationStatus.Paid &&
                             po.DateStarted < dateTime && po.DatePaid == null &&
                             po.CheckItems.Any(c => c.Status == CheckItemStatus.Unpaid))
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetFiscalizationErrorsInPurchaseModeIncludingPosOperationTransactionsSinceDateTimeAsync(
            DateTime dateTime)
        {
            return _context.PosOperations
                .Include(po => po.PosOperationTransactions)
                .Where(po => po.Mode == PosMode.Purchase &&
                             po.DateStarted < dateTime &&
                             po.PosOperationTransactions.Any(pot => pot.Status == PosOperationTransactionStatus.PaidUnfiscalized))
                .ToListAsync();
        }

        public Task<List<PosOperation>> GetPaidHavingCheckItemsByUserIdAsync(int userId)
        {
            return _context.PosOperations
                .Where(po => po.UserId == userId && po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Paid) &&
                             po.Status == PosOperationStatus.Paid)
                .ToListAsync();
        }

        public async Task<PosOperation> GetLastAdminPosOperationAsync(int posId)
        {
            return await _context.PosOperations
                .Where(p => p.PosId == posId && p.Mode != PosMode.Purchase)
                .OrderByDescending(p => p.DateStarted)
                .FirstOrDefaultAsync();
        }

        public Task<List<PosOperation>> GetUnhandledConditionalBeforeTimeAsync(DateTime purchasesSearchEndTime)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                .Where(po => po.Mode == PosMode.Purchase && po.DateCompleted < purchasesSearchEndTime &&
                             po.CheckItems.Any(ci => ci.Status == CheckItemStatus.PaidUnverified || ci.Status == CheckItemStatus.Unverified))
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, List<PosOperation>>> GetNutrientsByUserAsync(int userId, DateTime workDate, byte countOfDays)
        {
            var posOperationsDictionary = new Dictionary<DateTime, List<PosOperation>>();

            for (byte i = 0; i < countOfDays; i++)
            {
                var operationsSearchStartDateTime = workDate.AddDays(-i);
                var operationsSearchEndDateTime =
                    operationsSearchStartDateTime.Add(TimeSpan.FromTicks(TimeSpan.TicksPerDay));
                var posOperations = await GetPosOperationsByDateAsync(userId, operationsSearchStartDateTime, operationsSearchEndDateTime);
                posOperationsDictionary.Add(operationsSearchStartDateTime, posOperations);
            }

            return posOperationsDictionary;
        }

        public Task<List<PosOperation>> GetCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode mode, DateTimeRange dateRange)
        {
            return GetIncludingPosCheckItemsAndGoods()
                .Include(bt => bt.BankTransactionInfos)
                .Include(bt => bt.FiscalizationInfos)
                .OrderByDescending(po => po.DateCompleted)
                .Where(po =>
                    po.CheckItems.Any() &&
                    po.Mode == mode &&
                    po.DateCompleted != null &&
                    (po.DateCompleted.HasValue && po.DateCompleted.Value >= dateRange.Start && po.DateCompleted.Value < dateRange.End) ||
                    (po.DatePaid.HasValue && po.DatePaid.Value >= dateRange.Start && po.DatePaid.Value < dateRange.End) ||
                    (po.AuditCompletionDateTime.HasValue && po.AuditCompletionDateTime.Value >= dateRange.Start &&
                     po.AuditCompletionDateTime.Value < dateRange.End)
                ).ToListAsync();
        }

        public Task<List<PosOperation>> GetNewBillingCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode mode, DateTimeRange dateRange)
        {
            return GetIncludingPosCheckItemsAndGoods()
                .Include(pot => pot.PosOperationTransactions)
                    .ThenInclude(bt => bt.BankTransactionInfos)
                .Include(pot => pot.PosOperationTransactions)
                    .ThenInclude(fi => fi.FiscalizationInfos)
                .Include(pot => pot.PosOperationTransactions)
                    .ThenInclude(potcki => potcki.PosOperationTransactionCheckItems)
                .OrderByDescending(po => po.DateCompleted)
                .Where(po =>
                    po.CheckItems.Any() &&
                    po.Mode == mode &&
                    po.DateCompleted != null &&
                    (po.DateCompleted.HasValue && po.DateCompleted.Value >= dateRange.Start && po.DateCompleted.Value < dateRange.End) ||
                    (po.DatePaid.HasValue && po.DatePaid.Value >= dateRange.Start && po.DatePaid.Value < dateRange.End) ||
                    (po.AuditCompletionDateTime.HasValue && po.AuditCompletionDateTime.Value >= dateRange.Start &&
                     po.AuditCompletionDateTime.Value < dateRange.End)
                ).ToListAsync();
        }

        public Task<PosOperation> GetUnpaidCheckItemOfUserByPosOperationIdAsync(int userId, int posOperationId)
        {
            return GetCompletedUnpaidHavingUnpaidCheckItems()
                .Include(po => po.BankTransactionInfos)
                .Include(po => po.PosOperationTransactions)
                .ThenInclude(pot => pot.PosOperationTransactionCheckItems)
                .OrderBy(upt => upt.DateStarted)
                .FirstOrDefaultAsync(po =>
                    po.UserId == userId &&
                    po.DateCompleted != null &&
                    po.DatePaid == null &&
                    po.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unpaid) &&
                    po.Id == posOperationId);
        }

        public void Add(PosOperation posOperation)
        {
            _context.PosOperations.Add(posOperation);
        }

        public void Remove(PosOperation posOperation)
        {
            _context.PosOperations.Remove(posOperation);
        }

        private IQueryable<PosOperation> GetIncludingPosCheckItemsAndGoods()
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                .Include(po => po.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Good)
                .AsQueryable();
        }

        private Task<PosOperation> GetLatestOfUserByPredicateAsync(Expression<Func<PosOperation, bool>> predicate)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.LabeledGood)
                    .ThenInclude(lg => lg.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Good)
                    .ThenInclude(g => g.GoodCategory)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Currency)
                .Include(po => po.Pos)
                .Include(po => po.User)
                .Include(po => po.LabeledGoods)
                .Include(po => po.PosOperationTransactions)
                .OrderByDescending(upt => upt.DateStarted)
                .FirstOrDefaultAsync(predicate);
        }

        private IQueryable<PosOperation> GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Currency)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Good)
                    .ThenInclude(g => g.GoodCategory)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.LabeledGood)
                    .ThenInclude(cki => cki.Pos)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.Pos)
                .Include(po => po.PosDoorsStates)
                .Include(po => po.User)
                .Include(po => po.Pos)
                .AsQueryable();
        }

        private IQueryable<PosOperation> GetCompletedUnpaidHavingUnpaidCheckItems()
        {
            return GetIncludingCheckItemsWithLinkedRelationshipsPosAndUser()
                .Include(po => po.LabeledGoods)
                .Include(po => po.FiscalizationInfos)
                    .ThenInclude(fi => fi.FiscalizationCheckItems)
                .Include(po => po.CheckItems)
                    .ThenInclude(cki => cki.AuditRecords)
                .AsQueryable();
        }

        private Task<List<PosOperation>> GetPosOperationsByDateAsync(int userId, DateTime operationsSearchStartDateTime, DateTime operationsSearchEndDateTime)
        {
            return _context.PosOperations
                .Include(po => po.CheckItems)
                    .ThenInclude(ci => ci.Good)
                        .ThenInclude(g => g.ProteinsFatsCarbohydratesCalories)
                .Where(po => po.CheckItems.Any(ci => ci.Price > 0) &&
                             po.UserId == userId &&
                             po.DateCompleted != null &&
                             po.Mode == PosMode.Purchase &&
                             (po.Status == PosOperationStatus.Completed || 
                              po.Status == PosOperationStatus.Paid || 
                              po.Status == PosOperationStatus.PendingPayment) &&
                             operationsSearchStartDateTime <= po.DateCompleted.Value &&
                             po.DateCompleted.Value < operationsSearchEndDateTime)
                .OrderByDescending(po => po.DateCompleted)
                .ToListAsync();
        }
    }
}
