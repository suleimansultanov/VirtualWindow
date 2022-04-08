using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters
{
    public class PrintDocumentGoodsMovingUntiedLabeledGoodsFormatter : IPrintDocumentGoodsMovingUntiedLabeledGoodsFormatter
    {
        private readonly string _documentGoodsMovingPageUrlFormat;
        public PrintDocumentGoodsMovingUntiedLabeledGoodsFormatter(string documentGoodsMovingPageUrlFormat)
        {
            _documentGoodsMovingPageUrlFormat = documentGoodsMovingPageUrlFormat;
        }

        public string ApplyFormat(string printedLink, int posId)
        {
            return $"[{printedLink}]" +
                   $"({string.Format(_documentGoodsMovingPageUrlFormat, posId)})";
        }
    }
}
