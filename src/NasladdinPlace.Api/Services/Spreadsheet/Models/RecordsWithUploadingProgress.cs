using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Models
{
    public class RecordsWithUploadingProgress
    {
        public IEnumerable<IReportRecord> Records { get; }
        public SpreadsheetUploadingProgress UploadingProgress { get; }
        public Type TypeOfRecords { get; }

        public RecordsWithUploadingProgress(IEnumerable<IReportRecord> records, SpreadsheetUploadingProgress uploadingProgress)
        {
            Records = records;
            TypeOfRecords = Records.FirstOrDefault()?.GetType();
            UploadingProgress = uploadingProgress;
        }
    }
}
