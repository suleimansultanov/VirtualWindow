using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NasladdinPlace.Spreadsheets.Models;
using NasladdinPlace.Spreadsheets.Services.Spreadsheets.Contracts;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Color = Google.Apis.Sheets.v4.Data.Color;

namespace NasladdinPlace.Spreadsheets.Services.Spreadsheets
{
    public class GoogleSpreadsheetService : IGoogleSpreadsheetService
    {
        private readonly SheetsService _service;

        public GoogleSpreadsheetService(SheetsService service)
        {
            _service = service;
        }

        public async Task<ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>> CreateTableAsync(string title)
        {
            var requestBody = new Google.Apis.Sheets.v4.Data.Spreadsheet
            {
                Properties = new SpreadsheetProperties
                {
                    Title = title,
                },
            };

            var request = _service.Spreadsheets.Create(requestBody);

            var response = await request.ExecuteAsync();

            return
                response != null
                    ? ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>.Success(response)
                    : ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>.Failure(null);
        }

        public Task<ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>> GetSpreadsheetByIdAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return GetSpreadsheetByIdAuxAsync(id);
        }

        private async Task<ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>> GetSpreadsheetByIdAuxAsync(string id)
        {
            var request = _service.Spreadsheets.Get(id);
            request.IncludeGridData = false;

            var response = await request.ExecuteAsync();

            return
                response != null
                    ? ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>.Success(response)
                    : ValueResult<Google.Apis.Sheets.v4.Data.Spreadsheet>.Failure(null);
        }

        public Task<bool> TryFillAsync(UploadingSpreadsheetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return TryFillAuxAsync(info);
        }

        private async Task<bool> TryFillAuxAsync(UploadingSpreadsheetInfo info)
        {
            var appendRequest = CreateAppendRequest(info.CellData, info.SpreadsheetId, info.Range);

            var response = await appendRequest.ExecuteAsync();

            return response != null;
        }

        public Task TryFillWithFormatsAsync(UploadingSpreadsheetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return TryFillWithFormatsAuxAsync(info);
        }

        private async Task TryFillWithFormatsAuxAsync(UploadingSpreadsheetInfo info)
        {
            IList<RowData> rowData = info.CellFormats.Select(cells => new RowData()
            {
                Values = cells
            }).ToList();

            var request = new Request()
            {
                AppendCells = new AppendCellsRequest()
                {
                    Rows = rowData,
                    Fields = "*",
                    SheetId = info.SheetId
                }
            };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest()
            {
                Requests = new List<Request>() { request },
            };

            var updateRequest = _service.Spreadsheets.BatchUpdate(batchUpdateRequest, info.SpreadsheetId);

            var appendRequest = CreateAppendRequest(info.CellData, info.SpreadsheetId, info.Range);

            await updateRequest.ExecuteAsync();
            await appendRequest.ExecuteAsync();
        }

        public Task<bool> TryClearAsync(UploadingSpreadsheetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return TryClearAuxAsync(info);
        }

        private async Task<bool> TryClearAuxAsync(UploadingSpreadsheetInfo info)
        {
            var clearRequest = _service.Spreadsheets.Values.Clear(null, info.SpreadsheetId, info.Range);

            var response = await clearRequest.ExecuteAsync();

            return response != null;
        }

        public Task<bool> TryAddSheetAsync(string spreadsheetId, string title)
        {
            if (spreadsheetId == null)
                throw new ArgumentNullException(nameof(spreadsheetId));
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            return TryAddSheetAuxAsync(spreadsheetId, title);
        }

        private async Task<bool> TryAddSheetAuxAsync(string spreadsheetId, string title)
        {
            var requestBody = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

            requestBody.Requests.Add(new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = title
                    }
                }
            });

            requestBody.IncludeSpreadsheetInResponse = false;
            requestBody.ResponseIncludeGridData = false;

            var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);

            var response = await batchUpdateRequest.ExecuteAsync();

            return response.UpdatedSpreadsheet.Sheets.Select(s => s.Properties.Title)
                .Any(t => t?.Contains(title) ?? false);
        }

        public Task<bool> TryClearFiltersAsync(UploadingSpreadsheetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return TryClearFiltersAuxAsync(info);
        }

        private async Task<bool> TryClearFiltersAuxAsync(UploadingSpreadsheetInfo info)
        {
            var requestBody = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

            requestBody.Requests.Add(new Request
            {
                ClearBasicFilter = new ClearBasicFilterRequest
                {
                    SheetId = info.SheetId
                }
            });

            var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(requestBody, info.SpreadsheetId);

            var response = await batchUpdateRequest.ExecuteAsync();

            return response != null;
        }

        public Task<ValueResult<IEnumerable<IList<object>>>> ReadAsync(UploadingSpreadsheetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return ReadAuxAsync(info);
        }

        private async Task<ValueResult<IEnumerable<IList<object>>>> ReadAuxAsync(UploadingSpreadsheetInfo info)
        {
            var request = _service.Spreadsheets.Values.Get(info.SpreadsheetId, info.Range);

            var response = await request.ExecuteAsync();

            return
                response != null
                    ? ValueResult<IEnumerable<IList<object>>>.Success(response.Values)
                    : ValueResult<IEnumerable<IList<object>>>.Failure(null);
        }

        public Task<bool> TryAddCellWithUploadingProgressAsync(SpreadsheetUploadingProgressCell cell, UploadingSpreadsheetInfo info)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return TryAddCellWithUploadingProgressAuxAsync(cell, info);
        }

        private async Task<bool> TryAddCellWithUploadingProgressAuxAsync(SpreadsheetUploadingProgressCell cell, UploadingSpreadsheetInfo info)
        {
            IList<RowData> rowData = new List<RowData>()
            {
                new RowData()
                {
                    Values = new List<CellData>()
                    {
                        new CellData()
                        {
                            Note = cell.Note,
                            UserEnteredValue = new ExtendedValue()
                            {
                                StringValue = cell.Text
                            },
                            UserEnteredFormat = new CellFormat()
                            {
                                BackgroundColor = new Color()
                                {
                                    Red = GetColorAmountForCellBackground(cell.BackgroundColor.Red),
                                    Green = GetColorAmountForCellBackground(cell.BackgroundColor.Green),
                                    Blue = GetColorAmountForCellBackground(cell.BackgroundColor.Blue)
                                }
                            }
                        }
                    }
                }
            };

            var request = new Request()
            {
                UpdateCells = new UpdateCellsRequest()
                {
                    Fields = "*",
                    Range = new GridRange()
                    {
                        SheetId = info.SheetId,
                        StartColumnIndex = 0,
                        EndColumnIndex = 1,
                        EndRowIndex = 1,
                        StartRowIndex = 0
                    },
                    Rows = rowData,
                }
            };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest()
            {
                Requests = new List<Request>() { request },
            };

            var updateRequest = _service.Spreadsheets.BatchUpdate(batchUpdateRequest, info.SpreadsheetId);

            var response = await updateRequest.ExecuteAsync();

            return response != null;
        }

        private float? GetColorAmountForCellBackground(int colorFromRgb)
        {
            return colorFromRgb / byte.MaxValue;
        }

        private SpreadsheetsResource.ValuesResource.AppendRequest CreateAppendRequest(IEnumerable<IList<object>> data, string spreadsheetId, string range)
        {
            var valueBody = new ValueRange
            {
                Values = new List<IList<object>>()
            };

            foreach (var value in data)
            {
                valueBody.Values.Add(value);
            }

            var appendRequest =
                _service.Spreadsheets.Values.Append(valueBody, spreadsheetId, range);

            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            return appendRequest;
        }
    }
}
