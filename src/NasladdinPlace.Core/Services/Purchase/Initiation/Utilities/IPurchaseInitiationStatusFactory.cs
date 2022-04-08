using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;

namespace NasladdinPlace.Core.Services.Purchase.Initiation.Utilities
{
    public interface IPurchaseInitiationStatusFactory
    {
        PurchaseInitiationStatus Create(PosInteractionStatus posInteractionStatus);
    }
}