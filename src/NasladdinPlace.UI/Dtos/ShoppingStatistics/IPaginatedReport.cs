namespace NasladdinPlace.UI.Dtos.ShoppingStatistics
{
    public interface IPaginatedReport
    {
        int Page { get; set; }
        int TotalPages { get; set; }
        bool HasPreviousPage { get; set; }
        bool HasNextPage { get; set; }
    }
}
