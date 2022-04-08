namespace CloudPaymentsClient.Rest.Dtos.Shared
{
    public enum OperationStatusCode
    {
        AwaitingAuthentication = 1,
        Authorized = 2,
        Completed = 3,
        Cancelled = 4,
        Declined = 5
    }
}