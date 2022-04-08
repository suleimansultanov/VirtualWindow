using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Services.LabelsDetectionStatistics.Models
{
    public class LabelDetectionInfo
    { 
        public string Label { get; }
        public int ReadCount { get; set; }
        public int? LabeledGoodId { get; private set;  }
        public Good Good { get; private set; }

        public LabelDetectionInfo(string label, int readCount)
        {
            Label = label;
            ReadCount = readCount;
        }

        public void BindToLabeledGood(LabeledGood labeledGood)
        {
            LabeledGoodId = labeledGood.Id;
            Good = labeledGood.Good;
        }
    }
}