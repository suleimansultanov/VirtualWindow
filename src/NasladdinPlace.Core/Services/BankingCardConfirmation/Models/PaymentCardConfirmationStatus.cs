namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Models
{
    public enum PaymentCardConfirmationStatus
    {
        ConfirmationFailed = 0,
        ConfirmationSucceeded = 1,
        Require3DsAuthorization = 2,
        ConfirmationInitiated = 3,
        Authorization3DsCompletionInitiated = 4
    }
}