namespace NasladdinPlace.Core.Services.Shared.Models
{
    public class PaginationOptions
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public PaginationOptions(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}