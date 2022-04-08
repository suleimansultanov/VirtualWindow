namespace NasladdinPlace.UI.Dtos.User
{
    public class FilterDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string RegistrationInitiationDateFrom { get; set; }
        public string RegistrationInitiationDateUntil { get; set; }
        public string PaymentCardVerificationInitiationDateFrom { get; set; }
        public string PaymentCardVerificationInitiationDateUntil { get; set; }
        public string PaymentCardVerificationCompletionDateFrom { get; set; }
        public string PaymentCardVerificationCompletionDateUntil { get; set; }
        public string RegistrationCompletionDateFrom { get; set; }
        public string RegistrationCompletionDateUntil { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; } 
        public bool SortMethod { get; set; }

        public FilterDto()
        {
            Page = 1;
            PageSize = 25;
            SortBy = "Id";
            SortMethod = true;
        }
    }
}
