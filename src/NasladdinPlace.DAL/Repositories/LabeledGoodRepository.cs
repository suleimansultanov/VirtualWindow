using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.LabeledGoods;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.DAL.Repositories
{
    public class LabeledGoodRepository : Repository<LabeledGood>, ILabeledGoodRepository
    {
        public LabeledGoodRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<LabeledGood> GetEnabledAsync(int id)
        {
            return GetAllEnabled().SingleOrDefaultAsync(lg => lg.Id == id);
        }

        public Task<LabeledGood> GetIncludingGoodAndCurrencyByIdAsync(int labelGoodId)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .SingleOrDefaultAsync(lg => lg.Id == labelGoodId);
        }

        public Task<List<LabeledGood>> GetAllTiedIncludingGoodAndPosAndCurrencyAsync()
        {
            return CreateFilteredQueryFromSourceIncludingGoodAndPosAndCurrency(GetAll(), lg =>
                    lg.PosId != null
                    && lg.GoodId != null
                    && lg.CurrencyId != null
                    && lg.PosOperationId == null)
                .ToListAsync();
        }

        public List<LabeledGood> GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory()
        {
            return GetAll()
                .AsNoTracking()
                .Include(lg => lg.Good)
                .ThenInclude(g => g.GoodCategory)
                .Include(lg => lg.Pos)
                .Include(lg => lg.Currency)
                .Where(lg =>
                    lg.PosId != null && lg.GoodId != null && lg.CurrencyId != null && lg.PosOperationId == null)
                .OrderBy(lg => lg.Id)
                .ToList();
        }

        public Task<List<LabeledGood>> GetAllEnabledAsync()
        {
            return GetAllEnabled().ToListAsync();
        }

        public Task<List<LabeledGood>> GetByLabelsAsync(IEnumerable<string> labels)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .Where(lg => labels.Contains(lg.Label))
                .ToListAsync();
        }

        public Task<LabeledGood> GetByLabelAsync(string label)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .SingleOrDefaultAsync(lg => lg.Label == label);
        }

        public Task<List<LabeledGood>> GetEnabledByLabelsAsync(IEnumerable<string> labels)
        {
            return GetAllEnabled()
                .Include(lg => lg.Good)
                .Where(lg => labels.Contains(lg.Label))
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetEnabledInPosAsync(int posId)
        {
            return GetAllEnabled()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .Where(lg => lg.PosId == posId && lg.PosOperationId == null)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetInPosIncludingGoodAndCurrencyAsync(int posId)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .Where(lg => lg.PosId == posId && lg.PosOperationId == null)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetEnabledOverdueInPosAsync(int posId, TimeSpan beforeOverdueDelta)
        {
            return GetOverdueLabeledGoodsByContextQuery(GetAllEnabled(), beforeOverdueDelta)
                .Where(lg => lg.PosId == posId)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetEnabledInStockAsync()
        {
            return GetAllEnabled()
                .Include(lg => lg.Good)
                .Where(lg => !lg.PosId.HasValue)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetEnabledTakenByUserIncludingGoodsAndMakersAndPricesAsync(int userId)
        {
            return GetAllEnabled()
                .Include(lg => lg.Good).ThenInclude(g => g.Maker)
                .Include(lg => lg.Good).ThenInclude(g => g.GoodImages)
                .Include(lg => lg.Currency)
                .Where(lg => lg.PosOperation.UserId == userId &&
                             !lg.PosOperation.DateCompleted.HasValue)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetDisabledIncludingPosAndGoodsAsync()
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Pos)
                .Where(lg => lg.IsDisabled && lg.PosOperationId == null)
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .Where(lg => ids.Contains(lg.Id))
                .ToListAsync();
        }

        public Task<List<LabeledGood>> GetEnabledUntiedIncludingPosAsync()
        {
            return GetAll()
                .Include(lg => lg.Pos)
                .Where(lg => !lg.IsDisabled && 
                             lg.GoodId == null && 
                             lg.PosOperationId == null && 
                             lg.PosId != null &&
                             (lg.Pos.PosActivityStatus == PosActivityStatus.Active ||
                              lg.Pos.PosActivityStatus == PosActivityStatus.Test))
                .ToListAsync();
        }

        public Task<LabeledGood> GetByIdAsync(int id)
        {
            return GetAll().SingleOrDefaultAsync(lg => lg.Id == id);
        }

        public Task<List<LabeledGood>> GetEnabledUntiedFromGoodByPos(int posId)
        {
            return GetAllEnabled()
                .Where(lg => lg.PosId == posId && lg.GoodId == null && lg.PosOperationId == null)
                .ToListAsync();
        }

        public void AddRange(IEnumerable<LabeledGood> labeledGoods)
        {
            foreach (var labeledGood in labeledGoods)
            {
                Add(labeledGood);
            }
        }

        public IQueryable<LabeledGood> GetOverdueForDateTimeRange(DateTimeRange dateTimeRange)
        {
            return CreateFilteredQueryFromSourceIncludingGoodAndPosAndCurrency(GetAll(),
                lg => lg.PosId != null
                      && lg.GoodId != null
                      && lg.CurrencyId != null
                      && lg.PosOperationId == null
                      && (lg.ExpirationDate > dateTimeRange.Start &&
                          lg.ExpirationDate < dateTimeRange.End));
        }

        public Task<List<LabeledGood>> GetEnabledExceptUsedInPosOperationCheckItems(int posOperationId)
        {
            return Context.PosOperations.Where(pos => pos.Id == posOperationId)
                .SelectMany(pos => pos.Pos.LabeledGoods.Where(lg =>
                    lg.Price != null &&
                    lg.GoodId != null &&
                    lg.PosOperationId != posOperationId &&
                    pos.CheckItems.All(ci => ci.LabeledGoodId != lg.Id)))
                .Include(lg => lg.Good)
                .Include(lg => lg.Currency)
                .ToListAsync();
        }

        private static IQueryable<LabeledGood> GetOverdueLabeledGoodsByContextQuery(IQueryable<LabeledGood> context,
            TimeSpan beforeOverdueDelta)
        {
            var nowWithDelta = DateTime.UtcNow.Add(beforeOverdueDelta);

            return CreateFilteredQueryFromSourceIncludingGoodAndPosAndCurrency(context,
                    lg => lg.PosOperationId == null &&
                             lg.PosId != null &&
                             (
                                 lg.ExpirationDate == DateTime.MinValue ||
                                 lg.ExpirationDate != DateTime.MinValue &&
                                 lg.ExpirationDate < nowWithDelta
                             ) &&
                             lg.GoodId != null)
                .OrderBy(lg => lg.PosId)
                .ThenBy(lg => lg.ExpirationDate);
        }

        private static IQueryable<LabeledGood> CreateFilteredQueryFromSourceIncludingGoodAndPosAndCurrency(
            IQueryable<LabeledGood> query,
            Expression<Func<LabeledGood, bool>> filter)
        {
            return query.Include(lg => lg.Good)
                .Include(lg => lg.Pos)
                .Include(lg => lg.Currency)
                .Where(filter)
                .OrderBy(lg => lg.PosId)
                .ThenBy(lg => lg.ExpirationDate);
        }

        private IQueryable<LabeledGood> GetAllEnabled()
        {
            return GetAll().Where(lg => !lg.IsDisabled);
        }

        public Task<List<LabeledGood>> GetByIdsAndExpirationDateAsync(IEnumerable<int> ids, DateTime expirationDate)
        {
            return GetAll()
                .Where(lg => ids.Contains(lg.Id) && lg.ExpirationDate < expirationDate)
                .ToListAsync();
        }

        public IQueryable<LabeledGood> GetAllIncludingGood()
        {
            return GetAll()
                .Include(lg => lg.Good);
        }

        public Task<List<LabeledGood>> GetEnabledIncludingGoodInCategoryAsync(int posId, int categoryId, byte pageNumber, int pageSize)
        {
            return GetAll()
                .Include(lg => lg.Good)
                .ThenInclude(g => g.GoodCategory)
                .Include(lg => lg.Currency)
                .Include(lg => lg.Good)
                .ThenInclude(g => g.Maker)
                .Include(gc => gc.Good)
                .ThenInclude(g => g.ProteinsFatsCarbohydratesCalories)
                .Include(gc => gc.Good)
                .ThenInclude(g => g.GoodImages)
                .Where(lg => !lg.IsDisabled && lg.PosOperationId == null && lg.PosId != null &&
                             lg.Good.GoodCategoryId == categoryId && lg.PosId == posId)
                .ToAsyncEnumerable()
                .Distinct(new LabeledGoodComparer())
                .OrderBy(lg => lg.Good.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();
        }
    }
}