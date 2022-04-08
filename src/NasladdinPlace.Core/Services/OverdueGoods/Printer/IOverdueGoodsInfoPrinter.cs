using System.Collections.Generic;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer
{
    public interface IOverdueGoodsInfoPrinter: IMessagePrinter<Dictionary<OverdueType, IEnumerable<PosGoodInstances>>>
    { 
    }
}