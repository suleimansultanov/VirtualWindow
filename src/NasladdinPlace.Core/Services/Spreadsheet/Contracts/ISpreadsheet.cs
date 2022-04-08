using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Spreadsheet.Contracts
{
    public interface ISpreadsheet
    {
        Task FillAsync<TDataEntity>(IEnumerable<TDataEntity> data, string sheetTitle);
        Task FillAsync(IEnumerable<IList<object>> data, string sheetTitle);
        Task IsAllowedToEditAsync();
        Task ClearAsync(string sheetTitle);
        Task<IEnumerable<IList<object>>> ReadAsync(string sheetTitle);
        Task AddProgressNoteAsync(SpreadsheetUploadingProgress uploadingProgress, Type typeOfRecords, string sheetTitle);
    }
}
