using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Repositories.Utils;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IDocumentGoodsMovingRepository : IRepository<DocumentGoodsMoving>
    {
        Task<DocumentGoodsMoving> GetByIdIncludingTablePartAsync(int id);
        Task<DocumentGoodsMoving> GetByPosOperationIdIncludingTablePartPosOperationPosUserAndGoodAsync(int posOperationId);
        IEntityBatchesEnumerable<DocumentGoodsMoving> GetNotDeletedWithinPeriodIncludingTablePartGoodWithCategoryAndPos(
            int pageSize, DateTimeRange range);

        Task<List<DocumentGoodsMoving>> GetByDateRangeIncludingTablePartPosAndGoods(DateTimeRange dateRange);
    }
}
