using System.Collections.Generic;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Models;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Contracts
{
    public interface IUntiedLabeledGoodsInfoMessagePrinter
    {
        string Print(IEnumerable<UntiedLabeledGoodsInfo> untiedLabeledGoodsInfos);
        string PrintForGoodsMoving(UntiedLabeledGoodsInfo untiedLabeledGoodsInfo);
    }
}