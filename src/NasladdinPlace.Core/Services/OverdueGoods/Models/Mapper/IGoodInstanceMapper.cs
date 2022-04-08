using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models.Mapper
{
    public interface IGoodInstanceMapper
    {
        GoodInstance Transform(LabeledGood labeledGood);
        IEnumerable<GoodInstance> TransformCollection(IEnumerable<LabeledGood> labeledGoods);
    }
}