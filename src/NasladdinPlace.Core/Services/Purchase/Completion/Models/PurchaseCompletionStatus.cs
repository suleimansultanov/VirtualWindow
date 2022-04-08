namespace NasladdinPlace.Core.Services.Purchase.Completion.Models
{
    public enum PurchaseCompletionStatus
    {
        Success = 0,
        PaymentError = 1,
        UnknownError = 2,
        UnpaidPurchaseNotFound = 3,
        ProcessingPayment = 4
    }
}