using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosImageRepository : IRepository<PosImage>
    {
        IEnumerable<PosImage> GetByPos(int posId);
    }
}