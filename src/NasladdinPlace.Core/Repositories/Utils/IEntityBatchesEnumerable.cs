using System.Collections.Generic;
using PagedList.Interfaces;

namespace NasladdinPlace.Core.Repositories.Utils
{
    public interface IEntityBatchesEnumerable<out T> : IEnumerable<IPagedQueryable<T>>
    {
        int TotalItemsCount { get; }
    }
}