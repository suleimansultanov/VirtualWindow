using System.Linq;

namespace PagedList.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPagedQueryable<out T> : IPaginationInfo, IQueryable<T>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPagedQueryable : IPaginationInfo, IQueryable
    {
        
    }
}
