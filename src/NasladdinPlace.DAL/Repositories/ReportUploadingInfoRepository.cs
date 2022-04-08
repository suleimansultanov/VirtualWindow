using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class ReportUploadingInfoRepository : Repository<ReportUploadingInfo>, IReportUploadingInfoRepository
    {
        public ReportUploadingInfoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<ReportUploadingInfo>> GetAllAsync()
        {
            return Context.ReportsUploadingInfo.ToListAsync();
        }

        public Task<ReportUploadingInfo> GetReportsUploadingInfoByType(ReportType type)
        {
           return Context.ReportsUploadingInfo.SingleOrDefaultAsync(rp => rp.Type == type);
        }
    }
}
