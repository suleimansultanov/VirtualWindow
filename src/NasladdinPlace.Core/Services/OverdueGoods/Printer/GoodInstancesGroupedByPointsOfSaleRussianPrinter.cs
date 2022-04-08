using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters.Models;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer
{
    public class GoodInstancesGroupedByPointsOfSaleRussianPrinter : IGoodInstancesGroupedByPointsOfSalePrinter
    {
        private readonly IOrderedObjectStringFormatter<OverdueTypePosGoodInstances> _posGoodInstancesStringFormatter;

        private readonly StringBuilder _messageStringBuilder;

        public GoodInstancesGroupedByPointsOfSaleRussianPrinter(
            IOrderedObjectStringFormatter<OverdueTypePosGoodInstances> posGoodInstancesStringFormatter)
        {
            _posGoodInstancesStringFormatter = posGoodInstancesStringFormatter;
            _messageStringBuilder = new StringBuilder();
        }

        public void AddTitle(string header)
        {
            _messageStringBuilder.AppendLine(header);
        }

        public void AddEmptyLine()
        {
            _messageStringBuilder.AppendLine();
        }

        public void AddGoodInstancesGroupedByPointsOfSale(OverdueType overdueType, IEnumerable<PosGoodInstances> goodInstancesGroupedByPointsOfSale)
        {
            _messageStringBuilder.AppendLine(
                string.Join(
                    "  " + Environment.NewLine,
                    PrintEachPosGroup(overdueType, goodInstancesGroupedByPointsOfSale)
                )
            );
        }

        public string Print()
        {
            return _messageStringBuilder.ToString();
        }

        private IEnumerable<string> PrintEachPosGroup(OverdueType overdueType, IEnumerable<PosGoodInstances> posGoodInstances)
        {
            return posGoodInstances
                .Select(pgi => new OverdueTypePosGoodInstances(overdueType, pgi))
                .Select(_posGoodInstancesStringFormatter.ApplyFormat);
        }
    }
}