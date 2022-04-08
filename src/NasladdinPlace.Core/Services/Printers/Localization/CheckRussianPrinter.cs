using NasladdinPlace.Core.Services.Check.Simple.Models;
using System;
using System.Text;

namespace NasladdinPlace.Core.Services.Printers.Localization
{
    public class CheckRussianPrinter : ILocalizedPrinter<SimpleCheck>
    {
        public bool IncludeHeader { get; set; }

        public string Print(SimpleCheck entity)
        {
            var checkStringBuilder = new StringBuilder();
            var count = 0;
            if (IncludeHeader)
                checkStringBuilder.Append($"Чек:{Environment.NewLine}");
            foreach (var item in entity.Items)
            {
                var goodInfo = item.GoodInfo;
                var costSummary = item.CostSummary;
                var quantity = item.CostSummary.ItemsQuantity;
                checkStringBuilder.Append(
                    $"{++count}. {goodInfo.GoodName} {quantity} шт. {costSummary.CostWithoutDiscount} " +
                    $"{costSummary.Currency.Name}{Environment.NewLine}");
            }

            var checkCostSummary = entity.Summary.CostSummary;
            checkStringBuilder.Append($"Итого: {checkCostSummary.CostWithoutDiscount} " +
                                      $"{checkCostSummary.Currency.Name}{Environment.NewLine}");

            if (checkCostSummary.Discount <= 0) return checkStringBuilder.ToString();

            checkStringBuilder.Append($"Скидка: {checkCostSummary.Discount} {checkCostSummary.Currency.Name}{Environment.NewLine}");
            checkStringBuilder.Append($"Итого со скидкой: {checkCostSummary.CostWithDiscount} " +
                                      $"{checkCostSummary.Currency.Name}{Environment.NewLine}");

            return checkStringBuilder.ToString();
        }
    }
}
