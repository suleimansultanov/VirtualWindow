using System.Collections.Generic;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts
{
    public interface IReportDataBatchProvider
    {
        IEnumerable<RecordsWithUploadingProgress> Provide(int batchSize);
    }
}
