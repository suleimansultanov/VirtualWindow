namespace NasladdinPlace.Logging.PurchaseInitiating.Contracts
{
    public interface IPurchaseInitiationLogger
    {
        void LogStart(PurchaseInitiationPhase phase, string objectName = null, object initiatingObject = null);
        void LogFinish(PurchaseInitiationPhase phase, string objectName = null, object resultObject = null);
    }
}