using System;
using NasladdinPlace.Core.Models.Documents.Base;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Models.Documents.GoodsMoving
{
    public class DocumentGoodsMovingTableItem : DocumentTableItemBase<DocumentGoodsMoving>
    {
        public Good Good { get; private set; }
        public int? GoodId { get; private set; }
        public int BalanceAtBegining { get; private set; }
        public int BalanceAtEnd { get; private set; }
        public int Income { get; private set; }
        public int Outcome { get; private set; }
        public string LabelsAtBegining { get; private set; }
        public string LabelsAtEnd { get; private set; }

        protected DocumentGoodsMovingTableItem()
        {
            // intentionally left empty
        }

        public DocumentGoodsMovingTableItem(int lineNumber)
        {
            LineNum = lineNumber;
        }

        public void SetBalanceAndLabelsAtBegigning(GoodsMovingGrouppedItem item)
        {
            CheckOnNullAndSetGoodId(item);
            BalanceAtBegining = item.Count;
            LabelsAtBegining = item.Labels;
        }

        public void SetBalanceAndLabelsAtEnd(GoodsMovingGrouppedItem item)
        {
            CheckOnNullAndSetGoodId(item);
            BalanceAtEnd = item.Count;
            LabelsAtEnd = item.Labels;
        }

        public void SetIncome(GoodsMovingGrouppedItem item)
        {
            CheckOnNullAndSetGoodId(item);
            Income = item.Count;
        }

        public void SetOutcome(GoodsMovingGrouppedItem item)
        {
            CheckOnNullAndSetGoodId(item);
            Outcome = item.Count;
        }

        private void CheckOnNullAndSetGoodId(GoodsMovingGrouppedItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            GoodId = item.GoodId;
        }
    }
}
