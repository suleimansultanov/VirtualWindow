using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckLabeledGoodInfo
    {
        public string Label { get; }
        public int Id { get; }
        public DetailedCheckPosInfo PosInfo { get;}
        public bool IsInside { get; }
        public bool HasPosInfo { get; }
        public DateTime? FoundDateTime { get; }
        public DateTime? LostDateTime { get; }

        public DetailedCheckLabeledGoodInfo(LabeledGood labeledGood)
        {
            Label = labeledGood.Label;
            Id = labeledGood.Id;
            HasPosInfo = labeledGood.Pos != null;
            PosInfo = labeledGood.Pos != null
                ? new DetailedCheckPosInfo(labeledGood.Pos)
                : new DetailedCheckPosInfo(Core.Models.Pos.Default);
            IsInside = labeledGood.IsInsidePos;
            FoundDateTime = labeledGood.FoundDateTime;
            LostDateTime = labeledGood.LostDateTime;
        }
    }
}
