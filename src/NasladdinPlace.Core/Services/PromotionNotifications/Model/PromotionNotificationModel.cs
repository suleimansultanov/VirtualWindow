using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.PromotionNotifications.Model
{
    public class PromotionNotificationModel
    {
        public ApplicationUser User { get; set; }

        public string Message { get; set; }
    }
}
