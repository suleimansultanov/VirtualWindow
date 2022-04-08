using System;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.MobileAppsErrors.Models
{
    public class MobileAppError
    {
        public string Error { get; }
        public UserShortInfo UserShortInfo { get; }
        public AppInfo AppInfo { get; }
        public DeviceInfo DeviceInfo { get; }
        
        public string ErrorSource { get; set; }

        public MobileAppError(
            string error,
            UserShortInfo userShortInfo, 
            AppInfo appInfo,
            DeviceInfo deviceInfo)
        {
            if (string.IsNullOrEmpty(error))
                throw new ArgumentNullException(nameof(error));
            if (userShortInfo == null)
                throw new ArgumentNullException(nameof(userShortInfo));
            if (appInfo == null)
                throw new ArgumentNullException(nameof(appInfo));
            if (deviceInfo == null)
                throw new ArgumentNullException(nameof(deviceInfo));
            
            Error = error;
            UserShortInfo = userShortInfo;
            AppInfo = appInfo;
            DeviceInfo = deviceInfo;
        }
    }
}