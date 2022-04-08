namespace NasladdinPlace.Core.Services.Pos.Interactor.Models
{
    public enum PosInteractionStatus
    {
        Success = 0,
        LastPosOperationIncomplete = 1,
        PurchaseNotAllowed = 2,
        NoActiveOperationWithUser = 3,
        UnknownError = 4
    }
}