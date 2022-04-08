using System.Collections.Generic;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class DocumentGoodsMovingLabeledGoodRepository : Repository<DocumentGoodsMovingLabeledGood>, IDocumentGoodsMovingLabeledGoodRepository
    {
        public DocumentGoodsMovingLabeledGoodRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void AddRange(IEnumerable<DocumentGoodsMovingLabeledGood> documentGoodsMovingLabeledGoods)
        {
            foreach (var inventoryDocumentLabeled in documentGoodsMovingLabeledGoods)
            {
                Add(inventoryDocumentLabeled);
            }
        }
    }
}
