namespace NasladdinPlace.Application.Services.PosOperations.Contracts
{
    public interface IPosOperationsAppService
    {
        void ContinuePurchaseAsync(int userId);
        void InitiatePurchaseCompletionAsync(int userId);
    }
}