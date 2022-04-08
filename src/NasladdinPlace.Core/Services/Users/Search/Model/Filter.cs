namespace NasladdinPlace.Core.Services.Users.Search.Model
{
    public class Filter
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; }
        public OptionalDateTimeRange RegistrationInitiationDateRange { get; set; }
        public OptionalDateTimeRange PaymentCardVerificationInitiationDateRange { get; set; }
        public OptionalDateTimeRange PaymentCardVerificationCompletionDateRange { get; set; }
        public OptionalDateTimeRange RegistrationCompletionDateRange { get; set; }
    }
}
