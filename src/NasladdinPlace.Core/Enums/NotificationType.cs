using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    /// <summary>
    /// Type of send notification to user
    /// </summary>
    public enum NotificationType : byte
    {
        /// <summary>
        /// Sms notification
        /// </summary>
        [EnumDescription("Смс")]
        Sms = 0,

        /// <summary>
        /// Push notification
        /// </summary>
        [EnumDescription("Push уведомление")]
        Push = 1
    }
}
