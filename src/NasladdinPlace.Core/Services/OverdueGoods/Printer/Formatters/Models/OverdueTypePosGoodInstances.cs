using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters.Models
{
    public class OverdueTypePosGoodInstances
    {
        public OverdueType OverdueType { get; }
        public PosGoodInstances PosGoodInstances { get; }

        public OverdueTypePosGoodInstances(OverdueType overdueType, PosGoodInstances posGoodInstances)
        {
            OverdueType = overdueType;
            PosGoodInstances = posGoodInstances;
        }
    }
}