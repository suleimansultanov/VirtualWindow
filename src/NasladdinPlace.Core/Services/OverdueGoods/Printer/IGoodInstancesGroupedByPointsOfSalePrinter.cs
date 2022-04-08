using System.Collections.Generic;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer
{
    public interface IGoodInstancesGroupedByPointsOfSalePrinter
    {
        void AddTitle(string header);
        void AddEmptyLine();
        void AddGoodInstancesGroupedByPointsOfSale(
            OverdueType overdueType, IEnumerable<PosGoodInstances> goodInstancesGroupedByPointsOfSale
        );
        string Print();
    }
}