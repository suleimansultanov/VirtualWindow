using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters;
using NasladdinPlace.Core.Services.OverdueGoods.Printer.Formatters.Models;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Core.Services.OverdueGoods.Printer
{
    public class OverdueGoodsInfoRussianPrinter : IOverdueGoodsInfoPrinter
    {
        private const string OverdueGoodsTitle = "Срок годности истек: ";
        private const string OverdueGoodsInADayTitle = "Срок годности истекает завтра: ";
        private const string OverdueGoodsInATwoDaysTitle = "Срок годности истекает через 48 часов: ";
        private const string FreshGoodsTitle = "Нормальный срок годности: ";
        private const string NoGoodsFoundSecondPartTitle = "отсутствуют.";

        private static readonly Dictionary<OverdueType, string> OverdueGoodsTitleByType = new Dictionary<OverdueType, string>
        {
            [OverdueType.Overdue] = OverdueGoodsTitle,
            [OverdueType.OverdueInDay] = OverdueGoodsInADayTitle,
            [OverdueType.OverdueBetweenTommorowAndNextDay] = OverdueGoodsInATwoDaysTitle,
            [OverdueType.Fresh] = FreshGoodsTitle
        };

        private readonly IOrderedObjectStringFormatter<OverdueTypePosGoodInstances> _posGoodInstancesStringFormatter;

        public OverdueGoodsInfoRussianPrinter(
            IOrderedObjectStringFormatter<OverdueTypePosGoodInstances> posGoodInstancesStringFormatter)
        {
            _posGoodInstancesStringFormatter = posGoodInstancesStringFormatter;
        }

        public string Print(Dictionary<OverdueType, IEnumerable<PosGoodInstances>> entities)
        {
            var printer = new GoodInstancesGroupedByPointsOfSaleRussianPrinter(_posGoodInstancesStringFormatter);
            var sortedPosGoodInstancesGroupByOverdueType = new SortedDictionary<OverdueType, IEnumerable<PosGoodInstances>>(entities);

            foreach (var posGoodInstanceGroupByOverdueType in sortedPosGoodInstancesGroupByOverdueType)
            {
                OverdueGoodsTitleByType.TryGetValue(posGoodInstanceGroupByOverdueType.Key, out var overdueTypeTitle);

                if (entities.TryGetValue(posGoodInstanceGroupByOverdueType.Key, out var posGoodInstances) && posGoodInstances.Any())
                {
                    printer.AddTitle(overdueTypeTitle);
                    printer.AddGoodInstancesGroupedByPointsOfSale(posGoodInstanceGroupByOverdueType.Key,
                        posGoodInstanceGroupByOverdueType.Value);
                }
                else
                {
                    printer.AddTitle(overdueTypeTitle + NoGoodsFoundSecondPartTitle);
                }

                printer.AddEmptyLine();
            }

            return printer.Print();
        }
    }
}