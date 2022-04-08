using System;
using NasladdinPlace.Core.Services.Spreadsheet.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet;
using NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet.Contracts;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;
using NasladdinPlace.Spreadsheets.Services.Spreadsheets;

namespace NasladdinPlace.Spreadsheets.Providers
{
    public class SpreadsheetProvider : ISpreadsheetProvider
    {
        private readonly IGoogleSpreadsheetServiceFactory _googleGoogleSpreadsheetServiceFactory;
        private readonly ISpreadsheetIdFetcher _spreadsheetIdFetcher;
        private readonly ISpreadsheetCellFormatter _spreadsheetCellFormatter;
        private readonly ISpreadsheetDataRangeCreator _spreadsheetDataRangeCreator;

        public SpreadsheetProvider(
            IGoogleCredential googleCredential,
            ISpreadsheetIdFetcher spreadsheetIdFetcher,
            ISpreadsheetCellFormatter spreadsheetCellFormatter,
            ISpreadsheetDataRangeCreator spreadsheetDataRangeCreator,
            string applicationName)
        {
            if(googleCredential == null)
                throw new ArgumentNullException(nameof(googleCredential));
            if(spreadsheetIdFetcher == null)
                throw new ArgumentNullException(nameof(spreadsheetIdFetcher));
            if(spreadsheetCellFormatter == null)
                throw new ArgumentNullException(nameof(spreadsheetCellFormatter));
            if(spreadsheetDataRangeCreator == null)
                throw new ArgumentNullException(nameof(spreadsheetDataRangeCreator));
            if(applicationName == null)
                throw new ArgumentNullException(nameof(applicationName));

            _spreadsheetIdFetcher = spreadsheetIdFetcher;
            _spreadsheetCellFormatter = spreadsheetCellFormatter;
            _spreadsheetDataRangeCreator = spreadsheetDataRangeCreator;
            _googleGoogleSpreadsheetServiceFactory =
                new GoogleSpreadsheetServiceFactory(googleCredential, applicationName);
        }

        public ISpreadsheet Provide(string url)
        {
            if(url == null)
                throw new ArgumentNullException(nameof(url));

            var googleService =  _googleGoogleSpreadsheetServiceFactory.Create();

            return new Spreadsheet(googleService, _spreadsheetIdFetcher, _spreadsheetCellFormatter, _spreadsheetDataRangeCreator, url);
        }
    }
}
