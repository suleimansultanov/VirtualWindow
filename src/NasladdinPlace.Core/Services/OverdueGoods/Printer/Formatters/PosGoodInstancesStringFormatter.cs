using System;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters.Models;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters
{
    public class PosGoodInstancesStringFormatter : IOrderedObjectStringFormatter<OverdueTypePosGoodInstances>
    {
        private readonly string _adminSiteLinkFormat;

        public PosGoodInstancesStringFormatter(string adminSiteLinkFormat)
        {
            _adminSiteLinkFormat = adminSiteLinkFormat;
        }
        
        public string ApplyFormat(OverdueTypePosGoodInstances obj, int index)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            
            var overdueTypePosGoodInstances = obj;
            var posGoodInstances = overdueTypePosGoodInstances.PosGoodInstances;
            var posId = posGoodInstances.PosId;
            var overdueType = overdueTypePosGoodInstances.OverdueType;

            var posNumber = index + 1;
            var posName = posGoodInstances.PosName;
            var countInPos = posGoodInstances.Count;
            var adminSiteLink = string.Format(_adminSiteLinkFormat, posId, (int) overdueType);

            var formattedText = $"[Витрина №{posNumber} {posName} - {countInPos} штук.]" +
                                $"({adminSiteLink})";
            
            return formattedText;
        }
    }
}