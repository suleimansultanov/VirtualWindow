using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace NasladdinPlace.Spreadsheets.Models
{
    public class UploadingSpreadsheetInfo
    {
        public IEnumerable<IList<CellData>> CellFormats { get; }
        public IEnumerable<IList<object>> CellData { get; }
        public string SpreadsheetId { get; }
        public int? SheetId { get; }
        public string Range { get; }

        public UploadingSpreadsheetInfo(IEnumerable<IList<CellData>> cellFormats, IEnumerable<IList<object>> cellData, string spreadsheetId, int? sheetId, string range)
        {
            CellFormats = cellFormats;
            CellData = cellData;
            SpreadsheetId = spreadsheetId;
            SheetId = sheetId;
            Range = range;
        }

        public UploadingSpreadsheetInfo(string spreadsheetId, string range)
        {
            SpreadsheetId = spreadsheetId;
            Range = range;
        }

        public UploadingSpreadsheetInfo(string spreadsheetId, int? sheetId)
        {
            SpreadsheetId = spreadsheetId;
            SheetId = sheetId;
        }

        public UploadingSpreadsheetInfo(IEnumerable<IList<object>> cellData, string spreadsheetId, string range)
        {
            CellData = cellData;
            SpreadsheetId = spreadsheetId;
            Range = range;
        }
    }
}