using System.Collections.Generic;
using System.Collections.Immutable;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosImageRepository : Repository<PosImage>, IPosImageRepository
    {
        public PosImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<PosImage> GetByPos(int posId)
        {
            return Find(gi => gi.PosId == posId).ToImmutableList();
        }
    }
}