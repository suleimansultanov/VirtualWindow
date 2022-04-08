using System;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckEditingNotificationMessages
    {
        public NotificationMessage Refund { get; }
        public NotificationMessage AdditionOrVerification { get; }

        public CheckEditingNotificationMessages(
            NotificationMessage refund,
            NotificationMessage additionOrVerification)
        {
            if (refund == null)
                throw new ArgumentNullException(nameof(refund));
            if (additionOrVerification == null)
                throw new ArgumentNullException(nameof(additionOrVerification));

            Refund = refund;
            AdditionOrVerification = additionOrVerification;
        }
    }
}
