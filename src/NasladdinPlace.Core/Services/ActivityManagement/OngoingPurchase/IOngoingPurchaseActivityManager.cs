namespace NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase
{
    public interface IOngoingPurchaseActivityManager
    {
        IActivityManager<int> Users { get; }
        IActivityManager<int> PointsOfSale { get; }
    }
}