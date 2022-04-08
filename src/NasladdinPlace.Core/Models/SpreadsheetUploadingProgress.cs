using System;

namespace NasladdinPlace.Core.Models
{
    public class SpreadsheetUploadingProgress
    {
        public DateTime StartedAt { get; }
        public DateTime FinishedAt { get; }
        public double UploadingProgressInPercents { get; }
        public bool IsFinished { get; }

        public SpreadsheetUploadingProgress(DateTime uploadingStartedAt, int processedItemsCount, int totalItemsCount)
        {
            StartedAt = uploadingStartedAt;
            UploadingProgressInPercents = Math.Round(processedItemsCount / (double)totalItemsCount, 2);
            IsFinished = processedItemsCount == totalItemsCount;
            if(IsFinished) FinishedAt = DateTime.UtcNow;
        }
    }
}
