using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.OverdueGoods.Makers
{
    public interface IOverdueGoodsInfoMaker
    {
        Dictionary<OverdueType, IEnumerable<PosGoodInstances>> Make(ICollection<LabeledGood> pointsOfSaleContent);
    }
}