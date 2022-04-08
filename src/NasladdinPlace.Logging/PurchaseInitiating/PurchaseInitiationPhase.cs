using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Logging.PurchaseInitiating
{
    public enum PurchaseInitiationPhase
    {
        [EnumDescription("User balance calculation")]
        BalanceCalculation = 1,

        [EnumDescription("QR-Code encrypting")]
        QrCodeEncryption = 2,

        [EnumDescription("User activity tracking")]
        TrackingUserActivity = 3,

        [EnumDescription("Pos service purchase initiating")]
        PosServiceRequesting = 4,

        [EnumDescription("Purchase initiating handling")]
        PurchaseInitiatingResultHandling = 5,

        [EnumDescription("Purchase initiation finished successfully")]
        SuccessPurchaseInitiating = 6,

        [EnumDescription("Purchase initiation failed")]
        FailedPurchaseInitiating = 7,
    }
}