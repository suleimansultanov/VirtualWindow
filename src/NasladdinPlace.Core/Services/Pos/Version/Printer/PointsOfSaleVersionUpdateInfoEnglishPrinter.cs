using System;
using System.Linq;
using NasladdinPlace.Core.Services.Pos.Version.Models;
using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.Pos.Version.Printer
{
    public class PointsOfSaleVersionUpdateInfoEnglishPrinter : IMessagePrinter<PointsOfSaleVersionUpdateInfo>
    {
        public string Print(PointsOfSaleVersionUpdateInfo entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
            var pointsOfSaleVersionUpdateInfo = entities;
            var messageHeader = $"Version update up to {entities.RequiredMinVersion} is required for POS: ";
            var pointsOfSaleNames = pointsOfSaleVersionUpdateInfo.PointsOfSaleVersionInfo.Select(vi => vi.PosInfo.Name.Trim());
            var commaSeparatedPointsOfSale = string.Join(", ", pointsOfSaleNames);
            return $"{messageHeader} {commaSeparatedPointsOfSale}.";
        }
    }
}