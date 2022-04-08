using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PagedList.Interfaces;

namespace PagedList
{
    /// <summary>
    /// 	Represents a subset of a collection of objects that can be individually accessed by index and containing metadata about the superset collection of objects this subset was created from.
    /// </summary>
    /// <remarks>
    /// 	Represents a subset of a collection of objects that can be individually accessed by index and containing metadata about the superset collection of objects this subset was created from.
    /// </remarks>
    /// <typeparam name = "T">The type of object the collection should contain.</typeparam>
    /// <seealso cref = "IPagedList" />
    /// <seealso cref = "List{T}" />
    [Serializable]
    public class PagedQuery<T> : PagedListMetaData, IPagedQueryable<T>
    {
        /// <summary>
        /// 	The subset of items contained only within this one page of the superset.
        /// </summary>
        protected readonly IQueryable<T> Query;

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        protected internal PagedQuery(IQueryable<T> query, int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
            InitFields(query?.Count() ?? 0);

            if (query != null)
                Query = PageNumber == 1
                    ? query.Take(PageSize)
                    : query.Skip((PageNumber - 1) * PageSize).Take(PageSize);
        }


        #region Implementation of IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression => Query.Expression;

        /// <summary>
        /// 
        /// </summary>
	    public Type ElementType => Query.ElementType;

        /// <summary>
        /// 
        /// </summary>
	    public IQueryProvider Provider => Query.Provider;

        #endregion
    }
}
