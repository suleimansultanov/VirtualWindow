using System;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Models
{
    public class PosDiagnosticsState
    {
        public int PosId { get; private set; }
        public DateTime ContentSyncRequestDateTime { get; private set; }
        public DateTime DoorsStateSyncRequestDateTime { get; private set; }
        public bool IsAcceptableSyncTimeDelay { get; private set; }

        public PosDiagnosticsState(int posId)
        {
            PosId = posId;
            ContentSyncRequestDateTime = DateTime.UtcNow;
            DoorsStateSyncRequestDateTime = DateTime.UtcNow;
            IsAcceptableSyncTimeDelay = false;
        }

        public void MarkAsAcceptableSyncTimeDelay()
        {
            IsAcceptableSyncTimeDelay = true;
        }

        public void MarkAsUnacceptableSyncTimeDelay()
        {
            IsAcceptableSyncTimeDelay = false;
        }

    }
}
