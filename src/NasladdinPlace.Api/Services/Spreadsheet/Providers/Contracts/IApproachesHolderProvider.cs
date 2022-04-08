using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts
{
    public interface IApproachesHolderProvider
    {
        Task UploadOrCacheRecords(IEnumerable<IReportRecord> newRecords);
    }
}