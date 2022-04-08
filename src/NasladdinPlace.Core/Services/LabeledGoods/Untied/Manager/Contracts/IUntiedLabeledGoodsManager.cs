using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts
{
    public interface IUntiedLabeledGoodsManager
    {
        event EventHandler<IEnumerable<UntiedLabeledGoodsInfo>> OnUntiedLabeledGoodsFound;
        Task FindUntiedLabeledGoodsAsync();
    }
}