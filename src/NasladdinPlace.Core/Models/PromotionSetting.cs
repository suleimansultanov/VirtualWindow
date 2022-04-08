using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Core.Models
{
    public class PromotionSetting : Entity
    {
        public PromotionType PromotionType { get; private set; }
        public decimal BonusValue { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsNotificationEnabled { get; private set; }
        public TimeSpan NotificationStartTime { get; private set; }

        protected PromotionSetting()
        {
            // required for EF
        }

        public PromotionSetting(
            PromotionType promotionType,
            decimal bonusValue,
            TimeSpan notificationStartTime) : this()
        {
            Update(promotionType, bonusValue, notificationStartTime);
        }

        public PromotionSetting(
            PromotionType promotionType,
            decimal bonusValue,
            bool isEnabled, 
            bool isNotificationEnabled,
            TimeSpan notificationStartTime) 
            : this(promotionType, bonusValue, notificationStartTime)
        {
            IsEnabled = isEnabled;
            IsNotificationEnabled = isNotificationEnabled;
        }


        public void Disable()
        {
            IsEnabled = false;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void DisableNotification()
        {
            IsNotificationEnabled = false;
        }

        public void EnableNotification()
        {
            IsNotificationEnabled = true;
        }

        public void Update(PromotionType promotionType, decimal bonusValue, TimeSpan notificationStartTime)
        {
            if (bonusValue < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(bonusValue),
                    bonusValue,
                    $"{nameof(BonusValue)} should be over zero."
                );

            PromotionType = promotionType;
            BonusValue = bonusValue;
            NotificationStartTime = notificationStartTime;
        }
    }
}
