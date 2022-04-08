using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public class LabeledGoodsPrinter : ILabeledGoodsPrinter
    {
        public string Print(string title, IEnumerable<LabeledGood> labeledGoods)
        {
            var resultStringBuilder = new StringBuilder();

            var unknownGood = Good.Unknown;

            resultStringBuilder.AppendLine(title);
            var labeledGoodsStrings = labeledGoods
                .Select((lg, index) => $"{index + 1}. {(lg.Good ?? unknownGood).Name} - {lg.Label}")
                .ToImmutableList();
            var labeledGoodString = string.Join("\n", labeledGoodsStrings);
            resultStringBuilder.AppendLine(labeledGoodString);
            
            return resultStringBuilder.ToString();
        }
    }
}