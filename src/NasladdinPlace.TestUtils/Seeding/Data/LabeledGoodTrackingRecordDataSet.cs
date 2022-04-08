using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class LabeledGoodTrackingRecordDataSet : DataSet<LabeledGoodTrackingRecord>
    {
        private readonly int _labeledGoodId;
        private readonly int _posId;

        public LabeledGoodTrackingRecordDataSet(int labeledGoodId, int posId)
        {
            _labeledGoodId = labeledGoodId;
            _posId = posId;
        }

        protected override LabeledGoodTrackingRecord[] Data => new[]
        {
            LabeledGoodTrackingRecord.ForFoundLabeledGood(_labeledGoodId, _posId),
            LabeledGoodTrackingRecord.ForLostLabeledGood(_labeledGoodId, _posId),
        };
    }
}