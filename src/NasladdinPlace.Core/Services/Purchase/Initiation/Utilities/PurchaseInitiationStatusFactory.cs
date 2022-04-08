using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;

namespace NasladdinPlace.Core.Services.Purchase.Initiation.Utilities
{
    public class PurchaseInitiationStatusFactory : IPurchaseInitiationStatusFactory
    {
        public PurchaseInitiationStatus Create(PosInteractionStatus posInteractionStatus)
        {
            switch (posInteractionStatus)
            {
                case PosInteractionStatus.Success:
                    return PurchaseInitiationStatus.Success;
                case PosInteractionStatus.LastPosOperationIncomplete:
                    return PurchaseInitiationStatus.LastPosOperationIncomplete;
                case PosInteractionStatus.PurchaseNotAllowed:
                    return PurchaseInitiationStatus.PurchaseNotAllowed;
                case PosInteractionStatus.NoActiveOperationWithUser:
                case PosInteractionStatus.UnknownError:
                default:
                    return PurchaseInitiationStatus.InfrastructureFailure;
            }
        }
    }
}