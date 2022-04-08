using System.Linq;
using PagedList.Interfaces;

namespace PagedList
{
	/// <summary>
	/// 
	/// </summary>
	public static class PagedQueryExtensions
	{
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="query"></param>
        /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
        /// <param name="pageSize">The maximum size of any individual subset.</param>
        /// <returns>A subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.</returns>
        /// <seealso cref="PagedList{T}"/>
        public static IPagedQueryable<T> ToPagedQuery<T>(this IQueryable<T> query, int pageNumber, int pageSize)
		{
			return new PagedQuery<T>(query, pageNumber, pageSize);
		}
	}
}
