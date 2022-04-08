using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosLogsRepository : Repository<PosLog>, IPosLogsRepository
    {
        public PosLogsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<PosLog> GetAllIncludePos()
        {
            return  Context.PosLogs.Include(p => p.Pos).AsQueryable();
        }

        public bool CheckIsAnyById(int posLogId)
        {
            return Context.PosLogs.Select(p => p.Id).Any(l => l == posLogId);
        }

        public IEnumerable<PosLog> FindOldLogs(TimeSpan considerOldAfter)
        {
            return Context.PosLogs.Where(p => p.DateTimeCreated < DateTime.UtcNow.Add(-considerOldAfter)).ToList();
        }

        public void DeleteRange(IEnumerable<PosLog> logs)
        {
            Context.PosLogs.RemoveRange(logs);
        }
    }
}
