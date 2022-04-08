namespace NasladdinPlace.Api.Dtos.User
{
    public class FilterDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; }

        public FilterDto()
        {
            SortBy = "Id";
            Page = 1;
            PageSize = 25;
        }
    }
}
