using PagedList.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

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
    public abstract class BasePagedList<T> : PagedListMetaData, IPagedList<T>
    {
        /// <summary>
        /// 	The subset of items contained only within this one page of the superset.
        /// </summary>
        protected readonly List<T> Subset = new List<T>();


        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        protected BasePagedList()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        protected BasePagedList(int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItemCount"></param>
        protected BasePagedList(int pageNumber, int pageSize, int totalItemCount) : base(pageNumber, pageSize, totalItemCount)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return new List<T>(Subset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return Subset.ToArray();
        }


        #region IPagedList<T> Members

        /// <summary>
        /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
        /// </summary>
        /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Subset.GetEnumerator();
        }

        /// <summary>
        /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
        /// </summary>
        /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ///<summary>
        ///	Gets the element at the specified index.
        ///</summary>
        ///<param name = "index">The zero-based index of the element to get.</param>
        public T this[int index] => Subset[index];

        /// <summary>
        /// 	Gets the number of elements contained on this page.
        /// </summary>
        public int Count => Subset.Count;

        ///<summary>
        /// Gets a non-enumerable copy of this paged list.
        ///</summary>
        ///<returns>A non-enumerable copy of this paged list.</returns>
        public IPaginationInfo GetMetaData()
        {
            return new PagedListMetaData(this);
        }

        #endregion
    }
}
