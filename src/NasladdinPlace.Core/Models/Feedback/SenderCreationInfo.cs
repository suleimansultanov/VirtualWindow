using System;

namespace NasladdinPlace.Core.Models.Feedback
{
    public class SenderCreationInfo
    {
        public ApplicationUser User { get; }
        public string PhoneNumber { get; }
        public DeviceInfo DeviceInfo { get; }
        public bool IsSenderUnauthorized => User == null;

        public SenderCreationInfo(string phoneNumber, DeviceInfo deviceInfo)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));
            if (deviceInfo == null)
                throw new ArgumentNullException(nameof(deviceInfo));

            PhoneNumber = phoneNumber;
            DeviceInfo = deviceInfo;
        }

        public SenderCreationInfo(ApplicationUser user, DeviceInfo deviceInfo)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (deviceInfo == null)
                throw new ArgumentNullException(nameof(deviceInfo));

            User = user;
            DeviceInfo = deviceInfo;

            PhoneNumber = user.PhoneNumber;
        }
    }
}