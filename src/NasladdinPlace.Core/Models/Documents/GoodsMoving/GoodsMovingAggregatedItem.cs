using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models.Documents.GoodsMoving
{
    public class GoodsMovingAggregatedItem
    {
        public int? GoodId { get; private set; }
        public BalanceType Type { get; private set; }
        public LabeledGood LabeledGood { get; private set; }

        public GoodsMovingAggregatedItem(LabeledGood labeledGood, BalanceType balanceType)
        {
            if (labeledGood == null)
                throw new ArgumentNullException(nameof(labeledGood));
            if (!Enum.IsDefined(typeof(BalanceType), balanceType))
                throw new ArgumentException(nameof(balanceType));

            LabeledGood = labeledGood;
            GoodId = labeledGood.GoodId;
            Type = balanceType;
        }
    }
}
