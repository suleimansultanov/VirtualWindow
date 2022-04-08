using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Uploader.Contracts
{
    public interface IApproachesUploader
    {
        Task UploadAsync(IEnumerable<IReportRecord> records);
    }
}