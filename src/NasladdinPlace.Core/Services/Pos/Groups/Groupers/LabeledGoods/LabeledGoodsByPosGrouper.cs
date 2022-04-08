using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;

namespace NasladdinPlace.Core.Services.Pos.Groups.Groupers.LabeledGoods
{
    public class LabeledGoodsByPosGrouper : ILabeledGoodsByPosGrouper
    {
        public ICollection<PosGroup<LabeledGood>> Group(IEnumerable<LabeledGood> labeledGoods)
        {
            var immutableLabeledGoods = labeledGoods.ToImmutableList();
            var groupedLabeledGoodsByPosIdDictionary = new Dictionary<int, PosGroup<LabeledGood>>();

            foreach (var labeledGood in immutableLabeledGoods)
            {
                var posId = labeledGood.PosId ?? 0;
                
                var pos = labeledGood.Pos;
                var posInfo = pos == null ? PosShortInfo.Empty : PosShortInfo.FromPos(pos);
                
                if (!groupedLabeledGoodsByPosIdDictionary.ContainsKey(posId))
                {
                    groupedLabeledGoodsByPosIdDictionary[posId] = new PosGroup<LabeledGood>(posInfo);
                }
                
                groupedLabeledGoodsByPosIdDictionary[posId].AddItem(labeledGood);
            }

            return groupedLabeledGoodsByPosIdDictionary.Values
                .OrderBy(v => v.PosInfo.Name)
                .ThenBy(v => v.Items.Select(i => i.Good.Name))
                .ThenBy(v => v.Items.Select(i => i.Label))
                .ToImmutableList();
        }
    }
}