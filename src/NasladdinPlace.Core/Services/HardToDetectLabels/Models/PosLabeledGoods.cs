using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.HardToDetectLabels.Models
{
    public class PosLabeledGoods
    {
        public string PosName { get; }
        public ICollection<LabeledGood> LabeledGoods { get; }

        public PosLabeledGoods(string posName, IEnumerable<LabeledGood> labeledGoods)
        {
            PosName = posName;
            LabeledGoods = new List<LabeledGood>(labeledGoods);
        }

        public bool HasAnyLabeledGoods() => LabeledGoods.Any();
    }
}