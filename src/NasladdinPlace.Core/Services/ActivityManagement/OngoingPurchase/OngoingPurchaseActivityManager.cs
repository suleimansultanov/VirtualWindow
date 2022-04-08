namespace NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase
{
    public class OngoingPurchaseActivityManager : IOngoingPurchaseActivityManager
    {
        public OngoingPurchaseActivityManager(
            IActivityManager<int> userActivityManager,
            IActivityManager<int> posItemsManager)
        {
            Users = userActivityManager;
            PointsOfSale = posItemsManager;
        }

        public IActivityManager<int> Users { get; }
        public IActivityManager<int> PointsOfSale { get; }
    }
}