using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Repositories.Utils;
using PagedList.Interfaces;

namespace NasladdinPlace.DAL.Utils.EntityBatchesEnumeration
{
    public class EntityBatchesEnumerable<T> : IEntityBatchesEnumerable<T>
    {
        private readonly int _pageSize;
        private readonly IQueryable<T> _items;
        
        public int TotalItemsCount { get; }

        public EntityBatchesEnumerable(IQueryable<T> items, int pageSize)
        {
            _pageSize = pageSize;
            _items = items;
            TotalItemsCount = items.Count();
        }

        public IEnumerator<IPagedQueryable<T>> GetEnumerator()
        {
            return new EntityBatchesEnumerator<T>(_items, _pageSize);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}