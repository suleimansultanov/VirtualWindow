using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Core.Services.Pos.ContentSynchronization
{
    public class SyncResult
    {
        public IReadOnlyCollection<string> LabelsInPos { get; }
        public IReadOnlyCollection<string> LabelsOfPosInDatabase { get; }
        public IReadOnlyCollection<string> TakenLabels { get; }
        public IReadOnlyCollection<string> PutLabels { get; }

        public SyncResult(
            IEnumerable<string> labelsInPos, 
            IEnumerable<string> labelsOfPosInDatabase, 
            IEnumerable<string> takenLabels, 
            IEnumerable<string> putLabels)
        {
            LabelsInPos = labelsInPos.ToImmutableList();
            LabelsOfPosInDatabase = labelsOfPosInDatabase.ToImmutableList();
            TakenLabels = takenLabels.ToImmutableList();
            PutLabels = putLabels.ToImmutableList();
        }

        public bool IsSync => LabelsOfPosInDatabase.Intersect(PutLabels).Count() == PutLabels.Count && 
                              !TakenLabels.Intersect(LabelsOfPosInDatabase).Any();
    }
}