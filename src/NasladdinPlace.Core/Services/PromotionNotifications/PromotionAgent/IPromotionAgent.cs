using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.PromotionNotifications.Model;

namespace NasladdinPlace.Core.Services.PromotionNotifications.PromotionAgent
{
    public interface IPromotionAgent
    {
        event EventHandler<List<PromotionNotificationModel>> OnFoundPromotionNotifications;

        void Start(PromotionType promotionType, TimeSpan timeIntervalBetweenPromotion, TimeSpan startTime);

        void Stop(PromotionType promotionType);
    }
}
