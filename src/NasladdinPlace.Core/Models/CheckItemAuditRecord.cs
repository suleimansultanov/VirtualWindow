using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class CheckItemAuditRecord : Entity
    {
        public static CheckItemAuditRecord ForSystemOperation(
            int checkItemId, 
            CheckItemStatus oldStatus,
            CheckItemStatus newStatus)
        {
            return new CheckItemAuditRecord(checkItemId, oldStatus, newStatus, null);
        }

        public static CheckItemAuditRecord ForUserOperation(
            int checkItemId, 
            CheckItemStatus oldStatus,
            CheckItemStatus newStatus, 
            int editorId)
        {
            return new CheckItemAuditRecord(checkItemId, oldStatus, newStatus, editorId);
        }

        public int CheckItemId { get; private set; }
        public CheckItemStatus OldStatus { get; private set; }
        public CheckItemStatus NewStatus { get; private set; }

        public int? EditorId { get; private set; }
        public DateTime CreatedDate { get; private set;}

        public CheckItem CheckItem { get; private set; }
        public ApplicationUser User { get; private set; }

        protected CheckItemAuditRecord()
        {
            // required for EF
        }

        private CheckItemAuditRecord(int checkItemId, CheckItemStatus oldStatus, CheckItemStatus newStatus, int? editorId)
        {
            CheckItemId = checkItemId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            EditorId = editorId;
            CreatedDate = DateTime.UtcNow;
        }
    }
}