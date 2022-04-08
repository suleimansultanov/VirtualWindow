using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public interface ILabeledGoodsPrinter
    {
        string Print(string title, IEnumerable<LabeledGood> labeledGoods);
    }
}