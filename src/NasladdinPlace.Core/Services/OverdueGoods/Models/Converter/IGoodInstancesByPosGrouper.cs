using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models.Converter
{
    public interface IGoodInstancesByPosGrouper
    {
        IEnumerable<PosGoodInstances> Group(IEnumerable<GoodInstance> goodInstances);
    }
}