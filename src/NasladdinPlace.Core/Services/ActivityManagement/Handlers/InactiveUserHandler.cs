using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.ActivityManagement.Handlers
{
    public class InactiveUserHandler
    {
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;

        public InactiveUserHandler(IOngoingPurchaseActivityManager ongoingPurchaseActivityManager)
        {
            _ongoingPurchaseActivityManager = ongoingPurchaseActivityManager;
        }

        public Task Handle(int userId)
        {
            _ongoingPurchaseActivityManager.Users.StopTrackingActivity(userId);

            return Task.CompletedTask;
        }
    }
}