using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models.Reference;
using PagedList;
using PagedList.Interfaces;

namespace NasladdinPlace.DAL.Utils.EntityBatchesEnumeration
{
    public class EntityBatchesEnumerator<T> : IEnumerator<IPagedQueryable<T>>
    {
        private readonly PaginationInfo _paginationInfo;
        private readonly IQueryable<T> _items;

        public EntityBatchesEnumerator(IQueryable<T> items, int pageSize)
        {
            _paginationInfo = new PaginationInfo(0, pageSize, items.Count());
            _items = items;
        }

        public bool MoveNext()
        {
            return _paginationInfo.TryIncrementPage();
        }

        public void Reset()
        {
            _paginationInfo.ResetPage();
        }

        public IPagedQueryable<T> Current => _items.ToPagedQuery(_paginationInfo.Page, _paginationInfo.PageSize);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // intentionally left empty
        }

        ~EntityBatchesEnumerator()
        {
            Dispose(false);
        }
    }
}