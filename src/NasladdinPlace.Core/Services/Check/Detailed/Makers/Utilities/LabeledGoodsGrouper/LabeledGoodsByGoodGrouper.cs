using System.Collections.Generic;
using System.Collections.Immutable;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper.Contracts;

namespace NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper
{
    public class LabeledGoodsByGoodGrouper : ILabeledGoodsByGoodGrouper
    {
        public ICollection<Good> Group(ICollection<LabeledGood> labeledGoodsIncludingGoods)
        {
            var goodByIdDictionary = new Dictionary<int, Good>();
            
            foreach (var labeledGoodIncludingGood in labeledGoodsIncludingGoods)
            {
                var good = labeledGoodIncludingGood.Good ?? Good.Unknown;

                var goodId = good.Id;

                if (!goodByIdDictionary.ContainsKey(goodId))
                {
                    goodByIdDictionary[goodId] = good.Clone();
                }

                goodByIdDictionary[goodId].AddLabeledGood(labeledGoodIncludingGood);
            }

            return goodByIdDictionary.Values.ToImmutableList();
        }
    }
}