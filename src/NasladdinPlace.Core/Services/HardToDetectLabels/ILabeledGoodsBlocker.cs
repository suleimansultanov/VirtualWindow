using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public interface ILabeledGoodsBlocker
    {   
        Task BlockAsync();
        IEnumerable<LabeledGood> BlockedLabeledGoods { get; }
    }
}