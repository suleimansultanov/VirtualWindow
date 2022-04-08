using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface ILabeledGoodRepository : IRepository<LabeledGood>
    {
        Task<LabeledGood> GetEnabledAsync(int id);
        Task<LabeledGood> GetIncludingGoodAndCurrencyByIdAsync(int labelGoodId);
        Task<List<LabeledGood>> GetAllTiedIncludingGoodAndPosAndCurrencyAsync();
        List<LabeledGood> GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory();
        Task<List<LabeledGood>> GetInPosIncludingGoodAndCurrencyAsync(int posId);
        Task<LabeledGood> GetByIdAsync(int id);
        Task<List<LabeledGood>> GetAllEnabledAsync();
        Task<List<LabeledGood>> GetByLabelsAsync(IEnumerable<string> labels);
        Task<LabeledGood> GetByLabelAsync(string label);
        Task<List<LabeledGood>> GetEnabledByLabelsAsync(IEnumerable<string> labels);
        Task<List<LabeledGood>> GetEnabledInPosAsync(int posId);
        Task<List<LabeledGood>> GetEnabledOverdueInPosAsync(int posId, TimeSpan beforeOverdueDelta);
        Task<List<LabeledGood>> GetEnabledUntiedFromGoodByPos(int posId);
        Task<List<LabeledGood>> GetEnabledInStockAsync();
        Task<List<LabeledGood>> GetEnabledExceptUsedInPosOperationCheckItems(int posOperationId);
        Task<List<LabeledGood>> GetDisabledIncludingPosAndGoodsAsync();
        Task<List<LabeledGood>> GetByIdsAsync(IEnumerable<int> ids);
        Task<List<LabeledGood>> GetEnabledUntiedIncludingPosAsync();
        void AddRange(IEnumerable<LabeledGood> labeledGoods);
        Task<List<LabeledGood>> GetByIdsAndExpirationDateAsync(IEnumerable<int> ids, DateTime expirationDate);
        IQueryable<LabeledGood> GetOverdueForDateTimeRange(DateTimeRange dateTimeRange);
        IQueryable<LabeledGood> GetAllIncludingGood();
        Task<List<LabeledGood>> GetEnabledIncludingGoodInCategoryAsync(int posId, int categoryId, byte pageNumber, int pageSize);
    }
}