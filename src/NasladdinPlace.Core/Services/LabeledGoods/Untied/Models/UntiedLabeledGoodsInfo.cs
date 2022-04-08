using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Models
{
    public class UntiedLabeledGoodsInfo
    {
        public int PosId { get; }
        public int PosOperationId { get; }
        public string AbbreviatedName { get; }
        public int Count { get; }

        public UntiedLabeledGoodsInfo(int posId, string abbreviatedName, int count) : this(count)
        {
            PosId = posId;
            AbbreviatedName = abbreviatedName;
        }

        public UntiedLabeledGoodsInfo(PosOperation posOperation, int count) : this(count)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            PosOperationId = posOperation.Id;
            PosId = posOperation.PosId;
            AbbreviatedName = posOperation.GetAbbreviatedPosName();
        }

        private UntiedLabeledGoodsInfo(int count)
        {
            Count = count;
        }
    }
}