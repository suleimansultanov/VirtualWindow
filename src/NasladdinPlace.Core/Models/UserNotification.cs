using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Utils;
using System;

namespace NasladdinPlace.Core.Models
{
    public class UserNotification : Entity
    {
        private string _phoneNumber;
        private string _pushNotificationToken;
        private string _messageText;

        [Include]
        public ApplicationUser User { get; private set; }

        public int UserId { get; private set; }

        public string PhoneNumber {
            get => _phoneNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(PhoneNumber)} value must not be null.");

                _phoneNumber = value;
            }
        }

        public string PushNotificationToken {
            get => _pushNotificationToken;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(PushNotificationToken)} value must not be null.");

                _pushNotificationToken = value;
            }
        }

        public DateTime DateTimeSent { get; private set; }

        public string MessageText {
            get => _messageText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException($"{nameof(MessageText)} value must not be null.");

                _messageText = value;
            }
        }

        public NotificationType NotificationType { get; private set; }
        public NotificationArea NotificationArea { get; private set; }

        protected UserNotification()
        {
            DateTimeSent = DateTime.UtcNow;
        }

        public UserNotification(
            int userId,
            NotificationType notificationType,
            NotificationArea notificationArea) : this()
        {
            UserId = userId;
            NotificationType = notificationType;
            NotificationArea = notificationArea;
        }
    }
}
