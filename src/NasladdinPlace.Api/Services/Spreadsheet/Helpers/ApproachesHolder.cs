using System;
using System.Collections.Generic;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Helpers
{
    public class ApproachesHolder
    {
        public List<IReportRecord> Records { get; private set; }
        public DateTime ChangedDateTime { get; set; }

        public ApproachesHolder(DateTime changedDateTime)
        {
            Records = new List<IReportRecord>();
            ChangedDateTime = changedDateTime;
        }

        public void AddRecords(IEnumerable<IReportRecord> records)
        {
            if (Records == null)
            {
                Records = new List<IReportRecord>();
            }
            Records.AddRange(records);
            ChangedDateTime = DateTime.UtcNow;
        }
    }
}