using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts
{
    public interface ISpreadsheetUploadingProgressTracker
    {
        SpreadsheetUploadingProgress Track(int itemsCountInBatch);
    }
}