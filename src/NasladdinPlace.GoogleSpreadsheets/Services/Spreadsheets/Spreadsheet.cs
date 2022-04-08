using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Spreadsheet.Contracts;
using NasladdinPlace.Spreadsheets.Extensions;
using NasladdinPlace.Spreadsheets.Models;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;
using NasladdinPlace.Spreadsheets.Services.Spreadsheets.Contracts;

namespace NasladdinPlace.Spreadsheets.Services.Spreadsheets
{
    public class Spreadsheet : ISpreadsheet
    {
        private readonly IGoogleSpreadsheetService _service;
        private readonly ISpreadsheetIdFetcher _spreadsheetIdFetcher;
        private readonly ISpreadsheetCellFormatter _spreadsheetCellFormatter;
        private readonly ISpreadsheetDataRangeCreator _spreadsheetDataRangeCreator;

        public string Url { get; }
        public string SpreadsheetId => _spreadsheetIdFetcher.GetId(Url);

        public Spreadsheet(IGoogleSpreadsheetService service,
            ISpreadsheetIdFetcher spreadsheetIdFetcher,
            ISpreadsheetCellFormatter spreadsheetCellFormatter,
            ISpreadsheetDataRangeCreator spreadsheetDataRangeCreator,
            string url)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));
            if(spreadsheetIdFetcher == null)
                throw new ArgumentNullException(nameof(spreadsheetIdFetcher));
            if(spreadsheetCellFormatter == null)
                throw new ArgumentNullException(nameof(spreadsheetCellFormatter));
            if(spreadsheetDataRangeCreator == null)
                throw new ArgumentNullException(nameof(spreadsheetDataRangeCreator));
            if(url == null)
                throw new ArgumentNullException(nameof(url));

            _service = service;
            _spreadsheetIdFetcher = spreadsheetIdFetcher;
            _spreadsheetCellFormatter = spreadsheetCellFormatter;
            _spreadsheetDataRangeCreator = spreadsheetDataRangeCreator;
            Url = url;
        }

        public Task FillAsync<TDataEntity>(IEnumerable<TDataEntity> data, string sheetTitle)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (sheetTitle == null)
                throw new ArgumentNullException(nameof(sheetTitle));

            return FillAuxAsync(data, sheetTitle);
        }

        public Task FillAsync(IEnumerable<IList<object>> data, string sheetTitle)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (sheetTitle == null)
                throw new ArgumentNullException(nameof(sheetTitle));

            return FillAuxAsync(data, sheetTitle);
        }

        private async Task FillAuxAsync<TDataEntity>(IEnumerable<TDataEntity> data, string sheetTitle)
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            if (spreadsheetResult.Succeeded)
            {
                var sheets = spreadsheetResult.Value.Sheets.Where(s => s.Properties.Title.Equals(sheetTitle));

                var cellFormats = data.Select(r => _spreadsheetCellFormatter.GetCellFormats(r));

                var cellValues = data.Select(r => r.GetFieldsValues());

                var sheetId = sheets.FirstOrDefault()?.Properties.SheetId;

                var info = new UploadingSpreadsheetInfo(cellFormats, cellValues, SpreadsheetId, sheetId, _spreadsheetDataRangeCreator.Create(sheetTitle, cellValues.FirstOrDefault()?.Count));

                if (!sheets.Any())
                {
                    await _service.TryAddSheetAsync(SpreadsheetId, sheetTitle);
                    await _service.TryFillWithFormatsAsync(info);
                }
                else
                {
                    await _service.TryFillWithFormatsAsync(info);
                }
            }
        }

        private async Task FillAuxAsync(IEnumerable<IList<object>> data, string sheetTitle)
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            if (spreadsheetResult.Succeeded)
            {
                var sheets = spreadsheetResult.Value.Sheets.Where(s => s.Properties.Title.Equals(sheetTitle));

                var info = new UploadingSpreadsheetInfo(data, SpreadsheetId, _spreadsheetDataRangeCreator.Create(sheetTitle, data.FirstOrDefault()?.Count));

                if (!sheets.Any())
                {
                    await _service.TryAddSheetAsync(SpreadsheetId, sheetTitle);
                    await _service.TryFillAsync(info);
                }
                else
                {
                    await _service.TryFillAsync(info);
                }
            }
        }

        public Task ClearAsync(string sheetTitle)
        {
            if (sheetTitle == null)
                throw new ArgumentNullException(nameof(sheetTitle));

            return ClearAuxAsync(sheetTitle);
        }

        private async Task ClearAuxAsync(string sheetTitle)
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            if (spreadsheetResult.Succeeded)
            {
                var sheets = spreadsheetResult.Value.Sheets.Where(s => s.Properties.Title.Equals(sheetTitle));

                if (sheets.Any())
                {
                    var clearInfo = new UploadingSpreadsheetInfo(SpreadsheetId, _spreadsheetDataRangeCreator.Create(sheetTitle));

                    await _service.TryClearAsync(clearInfo);

                    var clearFiltersInfo = new UploadingSpreadsheetInfo(SpreadsheetId, sheets.FirstOrDefault().Properties.SheetId);

                    await _service.TryClearFiltersAsync(clearFiltersInfo);
                }

            }
        }

        public Task<IEnumerable<IList<object>>> ReadAsync(string sheetTitle)
        {
            if (sheetTitle == null)
                throw new ArgumentNullException(nameof(sheetTitle));

            return ReadAuxAsync(sheetTitle);
        }

        private async Task<IEnumerable<IList<object>>> ReadAuxAsync(string sheetTitle)
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            var sheets = spreadsheetResult.Value.Sheets.Where(s => s.Properties.Title.Equals(sheetTitle));

            if (!spreadsheetResult.Succeeded || !sheets.Any())
                return null;

            var info = new UploadingSpreadsheetInfo(SpreadsheetId, _spreadsheetDataRangeCreator.Create(sheetTitle));

            var readData = await _service.ReadAsync(info);

            return readData.Value;
        }

        public async Task IsAllowedToEditAsync()
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            if (!spreadsheetResult.Succeeded) return;

            var info = new UploadingSpreadsheetInfo(new List<IList<object>>(), SpreadsheetId, _spreadsheetDataRangeCreator.Create(spreadsheetResult.Value.Sheets.FirstOrDefault()?.Properties.Title));

            await _service.TryFillAsync(info);
        }

        public Task AddProgressNoteAsync(SpreadsheetUploadingProgress uploadingProgress, Type typeOfRecords, string sheetTitle)
        {
            if (uploadingProgress == null)
                throw new ArgumentNullException(nameof(uploadingProgress));
            if (sheetTitle == null)
                throw new ArgumentNullException(nameof(sheetTitle));

            return AddProgressNoteAuxAsync(uploadingProgress, typeOfRecords, sheetTitle);
        }

        private async Task AddProgressNoteAuxAsync(SpreadsheetUploadingProgress uploadingProgress, Type typeOfRecords, string sheetTitle)
        {
            var spreadsheetResult = await _service.GetSpreadsheetByIdAsync(SpreadsheetId);

            if (spreadsheetResult.Succeeded)
            {
                var sheets = spreadsheetResult.Value.Sheets.Where(s => s.Properties.Title.Equals(sheetTitle));

                if (sheets.Any())
                {
                    var info = new UploadingSpreadsheetInfo(SpreadsheetId, sheets.FirstOrDefault().Properties.SheetId);

                    var cellText = GetFirstFieldDisplayName(typeOfRecords);

                    var uploadingProgressCell = SpreadsheetUploadingProgressCell.Create(uploadingProgress, cellText);

                    await _service.TryAddCellWithUploadingProgressAsync(uploadingProgressCell, info);
                }
            }
        }

        private string GetFirstFieldDisplayName(Type reportRecordType)
        {
            string fieldDisplayName = string.Empty;

            if (reportRecordType == null)
                return fieldDisplayName;

            var firstField = reportRecordType.GetProperties().FirstOrDefault();

            if (firstField.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute)
            {
                fieldDisplayName = displayAttribute.Name;
            }

            return fieldDisplayName;
        }
    }
}
