using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper.Contracts
{
    public interface ILabeledGoodsByGoodGrouper
    {
        ICollection<Good> Group(ICollection<LabeledGood> labeledGoodsIncludingGoods);
    }
}