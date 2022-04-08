using System;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class NotificationMessage
    {
        public string MessageFormat { get; }
        public bool IsEnabled { get; }

        public NotificationMessage(string messageFormat, bool isEnabled)
        {
            if (string.IsNullOrEmpty(messageFormat))
                throw new ArgumentNullException(nameof(messageFormat));

            MessageFormat = messageFormat;
            IsEnabled = isEnabled;
        }
    }
}
