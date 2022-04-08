using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Disabled
{
    public interface IDisabledLabeledGoodsManager
    {
        Task<ICollection<PosGroup<LabeledGood>>> GetDisabledLabeledGoodsGroupedByPointsOfSaleAsync();
        Task EnableAsync(IEnumerable<int> labeledGoodIds);
    }
}