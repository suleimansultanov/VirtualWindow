using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Documents.Creators.Conctracts
{
    public interface IDocumentGoodsMovingCreator
    {
        Task<DocumentGoodsMoving> CreateAsync(
            IEnumerable<GoodsMovingAggregatedItem> labeledGoodsAtBegining,
            PosOperation posOperation,
            SyncResult syncResult,
            IUnitOfWork unitOfWork);
    }
}
