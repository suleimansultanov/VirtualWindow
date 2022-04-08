using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google;
using NasladdinPlace.Api.Services.Spreadsheet.Helpers;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Uploader
{
    public class ApproachesUploader : IApproachesUploader
    {
        private readonly ISpreadsheetProvider _spreadsheetProvider;
        private readonly SpreadsheetsUploadingTaskParameters _spreadsheetsUploadingTaskParameters;
        private readonly string _uri;
        private readonly string _sheet;

        public ApproachesUploader(
            ISpreadsheetProvider spreadsheetProvider,
            SpreadsheetsUploadingTaskParameters spreadsheetsUploadingTaskParameters,
            string uri,
            string sheet)
        {
            _spreadsheetProvider = spreadsheetProvider;
            _spreadsheetsUploadingTaskParameters = spreadsheetsUploadingTaskParameters;
            _uri = uri;
            _sheet = sheet;
        }

        public async Task UploadAsync(IEnumerable<IReportRecord> records)
        {
            var spreadsheet = _spreadsheetProvider.Provide(_uri);

            await RetryHelper.RetryOnExceptionAsync(_spreadsheetsUploadingTaskParameters.RetryDelay, 
                async () =>
                {
                    await spreadsheet.FillAsync(records, _sheet);
                }, IsKnownGoogleApiException,
                _spreadsheetsUploadingTaskParameters.PermittedRetryCount);
        }

        private static bool IsKnownGoogleApiException(Exception exception)
        {
            if (exception is GoogleApiException googleApiException)
            {
                return new[]
                {
                    (int) HttpStatusCode.InternalServerError,
                    (int) HttpStatusCode.ServiceUnavailable
                }.Contains(googleApiException.Error.Code);
            }

            return false;
        }
    }
}