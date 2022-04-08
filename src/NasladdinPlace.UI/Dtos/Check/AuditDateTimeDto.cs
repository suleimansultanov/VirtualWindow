using System;

namespace NasladdinPlace.UI.Dtos.Check
{
    public class AuditDateTimeDto
    {
        public DateTime? AuditRequestDateTime { get; set; }
        public DateTime? AuditCompletionDateTime { get; set; }
    }
}