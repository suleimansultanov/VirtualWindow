using System;
using System.Text;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Printers.Localization
{
    public class CheckEnglishPrinter : ILocalizedPrinter<SimpleCheck>
    {
        public bool IncludeHeader { get; set; }

        public string Print(SimpleCheck entity)
        {
            var checkStringBuilder = new StringBuilder();
            var count = 0;
            if (IncludeHeader)
                checkStringBuilder.Append($"Check:{Environment.NewLine}");
            foreach (var item in entity.Items)
            {
                var goodInfo = item.GoodInfo;
                var costSummary = item.CostSummary;
                var quantity = item.CostSummary.ItemsQuantity;
                checkStringBuilder.Append(
                    $"{++count}. {goodInfo.GoodName} {quantity} pc. {costSummary.CostWithoutDiscount} " +
                    $"{costSummary.Currency.Name}{Environment.NewLine}");
            }

            var checkCostSummary = entity.Summary.CostSummary;
            checkStringBuilder.Append($"Total: {checkCostSummary.CostWithoutDiscount} " +
                                      $"{checkCostSummary.Currency.Name}{Environment.NewLine}");

            if (checkCostSummary.Discount <= 0) return checkStringBuilder.ToString();

            checkStringBuilder.Append($"Discount: {checkCostSummary.Discount} {checkCostSummary.Currency.Name}{Environment.NewLine}");
            checkStringBuilder.Append($"Total with discount: {checkCostSummary.CostWithDiscount} " +
                                      $"{checkCostSummary.Currency.Name}{Environment.NewLine}");

            return checkStringBuilder.ToString();
        }
    }
}