using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.PromotionNotifications.Model;

namespace NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager
{
    public interface IPromotionManager
    {
        Task<List<PromotionNotificationModel>> GetPromotionNotificationsByType(PromotionType promotionType);

        Task<int> WritePromotionLog(PromotionNotificationModel notification, PromotionType promotionType, NotificationType notificationType);

        string BuildTaskName(PromotionType promotionType);
    }
}
