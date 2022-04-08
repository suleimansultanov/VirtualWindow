namespace NasladdinPlace.Core.Services.Purchase.Initiation.Models
{
    public enum PurchaseInitiationStatus
    {
        Success = 0,
        IncorrectQrCode = 1,
        NegativeBalance = 2,
        LastPosOperationIncomplete = 3,
        PurchaseNotAllowed = 4,
        InfrastructureFailure = 5
    }
}