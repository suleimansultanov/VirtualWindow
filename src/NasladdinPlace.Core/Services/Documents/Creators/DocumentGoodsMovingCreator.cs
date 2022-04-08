using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Documents.Creators.Conctracts;
using NasladdinPlace.Core.Services.Documents.Managers.Contracts;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.Documents.Creators
{
    public class DocumentGoodsMovingCreator: IDocumentGoodsMovingCreator
    {
        private readonly ILogger _logger;
        private readonly IDocumentManager<DocumentGoodsMovingTableItem> _documentManager;

        public DocumentGoodsMovingCreator(ILogger logger, IDocumentManager<DocumentGoodsMovingTableItem> documentManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (documentManager == null)
                throw new ArgumentNullException(nameof(documentManager));

            _logger = logger;
            _documentManager = documentManager;
        }

        public Task<DocumentGoodsMoving> CreateAsync(
            IEnumerable<GoodsMovingAggregatedItem> labeledGoodsAtBegining, 
            PosOperation posOperation, 
            SyncResult syncResult, 
            IUnitOfWork unitOfWork)
        {
            if(unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (syncResult == null)
                throw new ArgumentNullException(nameof(syncResult));

            return CreateAsyncAux(labeledGoodsAtBegining, posOperation, syncResult, unitOfWork);
        }

        private async Task<DocumentGoodsMoving> CreateAsyncAux(
            IEnumerable<GoodsMovingAggregatedItem> labeledGoodsAtBegining, 
            PosOperation posOperation, 
            SyncResult syncResult, 
            IUnitOfWork unitOfWork)
        {
            var documentGoodsMoving = new DocumentGoodsMoving(posOperation);

            var labeledGoodsAtEnd = (await unitOfWork.LabeledGoods.GetByLabelsAsync(syncResult.LabelsInPos))
                .Select(lg => new GoodsMovingAggregatedItem(lg, BalanceType.AtEnd)).ToImmutableList();

            var putLabeledGoods = (await unitOfWork.LabeledGoods.GetByLabelsAsync(syncResult.PutLabels))
                .Select(lg => new GoodsMovingAggregatedItem(lg, BalanceType.Income)).ToImmutableList();

            var takenLabeledGoods = (await unitOfWork.LabeledGoods.GetByLabelsAsync(syncResult.TakenLabels))
                .Select(lg => new GoodsMovingAggregatedItem(lg, BalanceType.Outcome)).ToImmutableList();

            var balanceLabeledGoods = labeledGoodsAtBegining.Concat(labeledGoodsAtEnd).ToImmutableList();
            var goodsMovingAggregatedItems = balanceLabeledGoods.Concat(putLabeledGoods).Concat(takenLabeledGoods).ToImmutableList();

            var documentGoodsMovingTableItems = CreateDocumentGoodsMovingTableItems(goodsMovingAggregatedItems);

            documentGoodsMoving.AddTablePartItems(documentGoodsMovingTableItems);

            if (documentGoodsMoving.GetItemWithUntiedLabeledGoods() != null)
                documentGoodsMoving.SetState(DocumentGoodsMovingState.HasUntiedGood);

            await _documentManager.SaveAsync(documentGoodsMoving, unitOfWork);

            try
            {
                var documentGoodsMovingLabeledGoods = CreateDocumentGoodsMovingLabeledGoods(documentGoodsMoving, balanceLabeledGoods);

                unitOfWork.DocumentGoodsMovingLabeledGoods.AddRange(documentGoodsMovingLabeledGoods);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while saving {nameof(DocumentGoodsMovingLabeledGood)}. Error: {ex}");
            }

            return documentGoodsMoving;
        }

        private static IEnumerable<DocumentGoodsMovingLabeledGood> CreateDocumentGoodsMovingLabeledGoods(DocumentGoodsMoving documentGoodsMoving,
            ImmutableList<GoodsMovingAggregatedItem> balanceLabeledGoods)
        {
            var documentGoodsMovingLabeledGoods = new List<DocumentGoodsMovingLabeledGood>();
            foreach (var documentGoodsMovingTableItem in documentGoodsMoving.TablePart)
            {
                documentGoodsMovingLabeledGoods
                    .AddRange(
                        balanceLabeledGoods
                            .Where(x => x.GoodId == documentGoodsMovingTableItem.GoodId)
                            .Select(y =>
                                new DocumentGoodsMovingLabeledGood(documentGoodsMovingTableItem, y.LabeledGood, y.Type))
                    );
            }

            return documentGoodsMovingLabeledGoods;
        }

        private static ICollection<DocumentGoodsMovingTableItem> CreateDocumentGoodsMovingTableItems(ImmutableList<GoodsMovingAggregatedItem> goodsMovingAggregatedItems)
        {
            var documentGoodsMovingTableItems = new Dictionary<int?, DocumentGoodsMovingTableItem>();
            var lineNumber = 1;

            foreach (var group in goodsMovingAggregatedItems.GroupBy(lg => new {GoodId = lg.GoodId ?? -1, lg.Type}).OrderBy(gr => gr.Key.GoodId))
            {
                var item = new GoodsMovingGrouppedItem
                    (group.Key.GoodId == -1 ? (int?)null : group.Key.GoodId, group.Count(), string.Join(",", group.Select(x => x.LabeledGood.Label)), group.Key.Type);

                if (!documentGoodsMovingTableItems.TryGetValue(group.Key.GoodId, out var documentGoodsMovingTableItem))
                {
                    documentGoodsMovingTableItem = new DocumentGoodsMovingTableItem(lineNumber++);
                    documentGoodsMovingTableItems.Add(group.Key.GoodId, documentGoodsMovingTableItem);
                }

                SetRequisites(item, documentGoodsMovingTableItem);
            }

            return documentGoodsMovingTableItems.Values;
        }

        private static void SetRequisites(GoodsMovingGrouppedItem grouppedItem, DocumentGoodsMovingTableItem goodsMovingTableItem)
        {
            switch (grouppedItem.Type)
            {
                case BalanceType.AtBegining:
                    goodsMovingTableItem.SetBalanceAndLabelsAtBegigning(grouppedItem);
                    break;
                case BalanceType.AtEnd:
                    goodsMovingTableItem.SetBalanceAndLabelsAtEnd(grouppedItem);
                    break;
                case BalanceType.Income:
                    goodsMovingTableItem.SetIncome(grouppedItem);
                    break;
                case BalanceType.Outcome:
                    goodsMovingTableItem.SetOutcome(grouppedItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(grouppedItem));
            }
        }
    }
}
