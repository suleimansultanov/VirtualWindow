using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet.Contracts;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Spreadsheets;
using NasladdinPlace.Spreadsheets.Services.Spreadsheets.Contracts;

namespace NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet
{
    public class GoogleSpreadsheetServiceFactory : IGoogleSpreadsheetServiceFactory
    {
        private readonly IGoogleCredential _googleCredential;
        private readonly string _applicationName;

        public GoogleSpreadsheetServiceFactory(IGoogleCredential googleCredential, string applicationName)
        {
            _googleCredential = googleCredential;
            _applicationName = applicationName;
        }

        public IGoogleSpreadsheetService Create()
        {
            var credentials = _googleCredential.CreateServiceAccountCredential();

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = _applicationName,
            });

            return new GoogleSpreadsheetService(service);
        }
    }
}
