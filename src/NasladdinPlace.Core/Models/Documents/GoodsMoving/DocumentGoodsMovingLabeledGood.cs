using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models.Documents.GoodsMoving
{
    public class DocumentGoodsMovingLabeledGood : Entity
    {
        public DocumentGoodsMovingTableItem DocumentGoodsMovingTableItem { get; private set; }
        public int DocumentTableItemId { get; private set; }
        public LabeledGood LabeledGood { get; private set; }
        public int LabeledGoodId { get; private set; }
        public BalanceType BalanceType { get; private set; }

        public DocumentGoodsMovingLabeledGood(int documentTableItemId, int labeledGoodId, BalanceType balanceType) : this(balanceType)
        {
            DocumentTableItemId = documentTableItemId;
            LabeledGoodId = labeledGoodId;
        }

        public DocumentGoodsMovingLabeledGood(DocumentGoodsMovingTableItem documentGoodsMovingTableItem, LabeledGood labeledGood, BalanceType balanceType): this(balanceType)
        {
            if (documentGoodsMovingTableItem == null)
                throw new ArgumentNullException(nameof(documentGoodsMovingTableItem));
            if (labeledGood == null)
                throw new ArgumentNullException(nameof(labeledGood));

            DocumentGoodsMovingTableItem = documentGoodsMovingTableItem;
            LabeledGood = labeledGood;
        }

        private DocumentGoodsMovingLabeledGood(BalanceType balanceType)
        {
            if (!Enum.IsDefined(typeof(BalanceType), balanceType))
                throw new ArgumentException(nameof(balanceType));

            BalanceType = balanceType;
        }
    }
}
