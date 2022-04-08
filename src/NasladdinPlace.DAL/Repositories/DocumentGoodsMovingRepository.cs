using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Repositories.Utils;
using NasladdinPlace.DAL.Utils.EntityBatchesEnumeration;
using NasladdinPlace.Utilities.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class DocumentGoodsMovingRepository : Repository<DocumentGoodsMoving>, IDocumentGoodsMovingRepository
    {
        public DocumentGoodsMovingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<DocumentGoodsMoving> GetByIdIncludingTablePartAsync(int id)
        {
            return GetAll()
                .Include(idoc => idoc.TablePart)
                .SingleOrDefaultAsync(idoc => idoc.Id == id);
        }

        public Task<DocumentGoodsMoving> GetByPosOperationIdIncludingTablePartPosOperationPosUserAndGoodAsync(int posOperationId)
        {
            return GetAll()
                .Include(idoc => idoc.TablePart).ThenInclude(ti => ti.Good)
                .Include(idoc => idoc.PosOperation).ThenInclude(po => po.User)
                .Include(idoc => idoc.PointOfSale)
                .SingleOrDefaultAsync(idoc => idoc.PosOperationId == posOperationId);
        }

        public IEntityBatchesEnumerable<DocumentGoodsMoving> GetNotDeletedWithinPeriodIncludingTablePartGoodWithCategoryAndPos(int pageSize, DateTimeRange range)
        {
            var documents = GetAll()
                .Include(d => d.PointOfSale)
                .Include(d => d.TablePart)
                .ThenInclude(tp => tp.Good)
                .ThenInclude(g => g.GoodCategory)
                .Where(d => !d.IsDeleted &&
                            d.CreatedDate >= range.Start &&
                            d.CreatedDate <= range.End)
                .OrderBy(d => d.PosId)
                .ThenBy(d => d.Id)
                .AsNoTracking();

            return new EntityBatchesEnumerable<DocumentGoodsMoving>(documents, pageSize);
        }

        public Task<List<DocumentGoodsMoving>> GetByDateRangeIncludingTablePartPosAndGoods(DateTimeRange dateRange)
        {
            return GetAll()
                .Include(idoc => idoc.TablePart)
                    .ThenInclude(ti => ti.Good)
                .Include(idoc => idoc.PointOfSale)
                .Where(doc => doc.CreatedDate >= dateRange.Start && doc.CreatedDate <= dateRange.End)
                .ToListAsync();
        }
    }
}
