using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.UI.Dtos.ShoppingStatistics
{
    public class PaginatedReportDto<T> : IPaginatedReport
    {
        public ICollection<T> Content { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PaginatedReportDto()
        {
            Content = new Collection<T>();
        }
    }
}
