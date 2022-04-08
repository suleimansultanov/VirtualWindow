using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosLogsRepository : IRepository<PosLog>
    {
        IQueryable<PosLog> GetAllIncludePos();

        bool CheckIsAnyById(int posLogId);

        IEnumerable<PosLog> FindOldLogs(TimeSpan considerOldAfter);

        void DeleteRange(IEnumerable<PosLog> logs);
    }
}
