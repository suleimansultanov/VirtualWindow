using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class PromotionLog : Entity
    {
        public ApplicationUser User { get; private set; }

        public int UserId { get; private set; }
        public PromotionType PromotionType { get; private set; }
        public DateTime SendMessageDateTime { get; private set; }
        public NotificationType NotificationType { get; private set; }

        protected PromotionLog()
        {
            SendMessageDateTime = DateTime.UtcNow;
        }

        public PromotionLog(int userId, PromotionType promotionType, NotificationType notificationType) 
            : this()
        {
            UserId = userId;
            PromotionType = promotionType;
            NotificationType = notificationType;
        }
    }
}
