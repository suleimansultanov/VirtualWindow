using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models.Documents.GoodsMoving
{
    public class GoodsMovingGrouppedItem
    {
        public int? GoodId { get; private set; }
        public int Count { get; private set; }
        public string Labels { get; private set; }
        public BalanceType Type { get; private set; }

        public GoodsMovingGrouppedItem(int? goodId, int count, string labels, BalanceType balanceType)
        {
            if (labels == null)
                throw new ArgumentNullException(nameof(labels));
            if (!Enum.IsDefined(typeof(BalanceType), balanceType))
                throw new ArgumentException(nameof(balanceType));

            GoodId = goodId;
            Count = count;
            Labels = labels;
            Type = balanceType;
        }
    }
}
