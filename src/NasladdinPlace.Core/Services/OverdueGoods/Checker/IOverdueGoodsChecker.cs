using NasladdinPlace.Core.Services.OverdueGoods.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Core.Services.OverdueGoods.Checker
{
    public interface IOverdueGoodsChecker
    {
        event EventHandler<Dictionary<OverdueType, IEnumerable<PosGoodInstances>>> OnFoundOverdueGoods;

        Task CheckAsync();
    }
}