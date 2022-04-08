using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class LabeledGoodTrackingRecord : Entity
    {
        public static LabeledGoodTrackingRecord ForLostLabeledGood(int labeledGoodId, int posId)
        {
            return new LabeledGoodTrackingRecord(
                labeledGoodId,
                posId,
                LabeledGoodTrackingRecordType.Lost
            );
        }

        public static LabeledGoodTrackingRecord ForFoundLabeledGood(int labeledGoodId, int posId)
        {
            return new LabeledGoodTrackingRecord(
                labeledGoodId,
                posId,
                LabeledGoodTrackingRecordType.Found
            );
        }
        
        public LabeledGood LabeledGood { get; private set; }
        public Pos Pos { get; private set; }
        
        public int LabeledGoodId { get; private set; }
        public int PosId { get; private set; }
        public LabeledGoodTrackingRecordType Type { get; private set; }
        public DateTime Timestamp { get; private set; }

        protected LabeledGoodTrackingRecord()
        {
            Timestamp = DateTime.UtcNow;
        }

        private LabeledGoodTrackingRecord(
            int labeledGoodId, 
            int posId,
            LabeledGoodTrackingRecordType type) : this()
        {
            LabeledGoodId = labeledGoodId;
            PosId = posId;
            Type = type;
        }
    }
}