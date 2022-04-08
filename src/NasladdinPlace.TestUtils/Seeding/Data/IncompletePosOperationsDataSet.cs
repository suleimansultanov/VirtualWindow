using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class IncompletePosOperationsDataSet : DataSet<PosOperation>
    {
        private readonly int _posId;
        private readonly int _userId;
        public IncompletePosOperationsDataSet(int posId, int userId)
        {
            _posId = posId;
            _userId = userId;
        }
        
        protected override PosOperation[] Data => new[]
        {
            PosOperation.NewOfUserAndPosBuilder(_userId, _posId)
            .SetDateStarted(DateTime.UtcNow.AddMinutes(-100))
            .SetDateSentForVerification(DateTime.UtcNow.AddMinutes(-99))
            .Build()
        };
    }
}