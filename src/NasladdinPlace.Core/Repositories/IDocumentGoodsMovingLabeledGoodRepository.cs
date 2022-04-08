using System.Collections.Generic;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;

namespace NasladdinPlace.Core.Repositories
{
    public interface IDocumentGoodsMovingLabeledGoodRepository : IRepository<DocumentGoodsMovingLabeledGood>
    {
        void AddRange(IEnumerable<DocumentGoodsMovingLabeledGood> documentGoodsMovingLabeledGoods);
    }
}
