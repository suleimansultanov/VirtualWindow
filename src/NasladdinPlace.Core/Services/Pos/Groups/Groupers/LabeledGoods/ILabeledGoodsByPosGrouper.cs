using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;

namespace NasladdinPlace.Core.Services.Pos.Groups.Groupers.LabeledGoods
{
    public interface ILabeledGoodsByPosGrouper
    {
        ICollection<PosGroup<LabeledGood>> Group(IEnumerable<LabeledGood> labeledGoods);
    }
}