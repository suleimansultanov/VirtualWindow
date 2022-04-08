using System.Collections.Generic;
using System.Text;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Models;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer
{
    public class UntiedLabeledGoodsInfoRussianPrinter : IUntiedLabeledGoodsInfoMessagePrinter
    {
        private readonly IPrintedUntiedLabeledGoodsFormatter _printedUntiedLabeledGoodsFormatter;
        private readonly IPrintDocumentGoodsMovingUntiedLabeledGoodsFormatter _printDocumentGoodsMovingUntiedLabeledGoodsFormatter;

        public UntiedLabeledGoodsInfoRussianPrinter(
            IPrintedUntiedLabeledGoodsFormatter printedUntiedLabeledGoodsFormatter,
            IPrintDocumentGoodsMovingUntiedLabeledGoodsFormatter printDocumentGoodsMovingUntiedLabeledGoodsFormatter)
        {
            _printedUntiedLabeledGoodsFormatter = printedUntiedLabeledGoodsFormatter;
            _printDocumentGoodsMovingUntiedLabeledGoodsFormatter = printDocumentGoodsMovingUntiedLabeledGoodsFormatter;
        }

        public string Print(IEnumerable<UntiedLabeledGoodsInfo> untiedLabeledGoodsInfos)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"{Emoji.Link} [[Логисту]] Непривязанные метки:");

            foreach (var untiedLabeledGoodsInfo in untiedLabeledGoodsInfos)
                CreateLinkMessage(_printedUntiedLabeledGoodsFormatter, messageBuilder, untiedLabeledGoodsInfo);

            messageBuilder.AppendLine("Закончите идентификацию товара");

            return messageBuilder.ToString();
        }

        public string PrintForGoodsMoving(UntiedLabeledGoodsInfo untiedLabeledGoodsInfo)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"{Emoji.No_Entry} [[Логисту]] Непривязанные метки:");

            CreateLinkMessage(_printDocumentGoodsMovingUntiedLabeledGoodsFormatter, messageBuilder, untiedLabeledGoodsInfo);

            messageBuilder.AppendLine("При размещении найдены непривязанные метки");

            return messageBuilder.ToString();
        }

        private static void CreateLinkMessage(IBaseFormatter linkFormatter, StringBuilder messageBuilder, UntiedLabeledGoodsInfo untiedLabeledGoodsInfo)
        {
            messageBuilder.AppendLine(
                linkFormatter.ApplyFormat(
                    $"Витрина {untiedLabeledGoodsInfo.AbbreviatedName} - {untiedLabeledGoodsInfo.Count} шт.",
                    linkFormatter is IPrintedUntiedLabeledGoodsFormatter ?  untiedLabeledGoodsInfo.PosId : untiedLabeledGoodsInfo.PosOperationId));
        }
    }
}