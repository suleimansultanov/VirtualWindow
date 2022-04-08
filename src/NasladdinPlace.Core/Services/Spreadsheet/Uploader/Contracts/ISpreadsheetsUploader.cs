using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Models;

namespace NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts
{
    public interface ISpreadsheetsUploader
    {
        event EventHandler<ReportUploadingError> ErrorHandler;
        Task UploadAsync(ReportType reportType);
    }
}
