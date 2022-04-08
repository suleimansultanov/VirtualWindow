using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters
{
    public class PrintedUntiedLabeledGoodsFormatter : IPrintedUntiedLabeledGoodsFormatter
    {
        private readonly string _administrationIdentificationsLinkFormat;

        public PrintedUntiedLabeledGoodsFormatter(string administrationIdentificationsLinkFormat)
        {
            _administrationIdentificationsLinkFormat = administrationIdentificationsLinkFormat;
        }

        public string ApplyFormat(string printedLink, int posId)
        {
            return $"[{printedLink}]" +
                   $"({string.Format(_administrationIdentificationsLinkFormat, posId)})";
        }
    }
}