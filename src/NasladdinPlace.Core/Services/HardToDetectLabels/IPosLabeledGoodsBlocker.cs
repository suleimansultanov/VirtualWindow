using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.HardToDetectLabels.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public interface IPosLabeledGoodsBlocker
    {
        event EventHandler<PosLabeledGoods> LabeledGoodsBlocked;
        Task BlockAsync(IUnitOfWork unitOfWork, PosContent posContentToBlock);
    }
}