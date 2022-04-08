using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Dtos.Audit
{
    public class CheckItemAuditRecordDto
    {
        public string GoodName { get; set; }
        public string Label { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public CheckItemStatus OldStatus { get; private set; }
        public CheckItemStatus NewStatus { get; private set; }
    }
}
