using System;
using NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Creators
{
    public class SpreadsheetUploadingProgressTracker : ISpreadsheetUploadingProgressTracker
    {
        private readonly DateTime _uploadingStartedAt;
        private readonly int _totalItemsCount;

        private int _processedItemsCount;

        public SpreadsheetUploadingProgressTracker(int totalItemsCount)
        {
            _uploadingStartedAt = DateTime.UtcNow;
            _totalItemsCount = totalItemsCount;
        }

        public SpreadsheetUploadingProgress Track(int itemsCountInBatch)
        {
            _processedItemsCount += itemsCountInBatch;
            return new SpreadsheetUploadingProgress(_uploadingStartedAt, _processedItemsCount, _totalItemsCount);
        }
    }
}
