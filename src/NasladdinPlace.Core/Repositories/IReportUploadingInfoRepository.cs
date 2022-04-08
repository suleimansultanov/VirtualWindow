using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IReportUploadingInfoRepository : IRepository<ReportUploadingInfo>
    {
        Task<ReportUploadingInfo> GetReportsUploadingInfoByType(ReportType type);
        Task<List<ReportUploadingInfo>> GetAllAsync();
    }
}
