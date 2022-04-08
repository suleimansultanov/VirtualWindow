using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.MobileAppsErrors.Models;
using NasladdinPlace.Core.Services.Printers.Common;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Printers
{
    public class MobileAppErrorPrinter : BaseMessagePrinter<MobileAppError>
    {
        private readonly IMessagePrinter<UserShortInfo> _userShortInfoRussianPrinter;
        private readonly IMessagePrinter<AppInfo> _appInfoMessagePrinter;
        private readonly IMessagePrinter<DeviceInfo> _deviceInfoMessagePrinter;

        public MobileAppErrorPrinter(
            IMessagePrinter<UserShortInfo> userShortInfoRussianPrinter,
            IMessagePrinter<AppInfo> appInfoMessagePrinter,
            IMessagePrinter<DeviceInfo> deviceInfoMessagePrinter)
        {
            _userShortInfoRussianPrinter = userShortInfoRussianPrinter;
            _appInfoMessagePrinter = appInfoMessagePrinter;
            _deviceInfoMessagePrinter = deviceInfoMessagePrinter;
        }

        protected override IEnumerable<string> ProvideMessagePieces(MobileAppError entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            var mobileAppError = entity;
            return new[]
            {
                $"Детали ошибки: {mobileAppError.Error}",
                $"Экран: {GetScreenFromError(mobileAppError)}",
                _userShortInfoRussianPrinter.Print(mobileAppError.UserShortInfo),
                _deviceInfoMessagePrinter.Print(mobileAppError.DeviceInfo),
                _appInfoMessagePrinter.Print(mobileAppError.AppInfo)
            };
        }

        private static string GetScreenFromError(MobileAppError mobileAppError)
        {
            return string.IsNullOrEmpty(mobileAppError.ErrorSource) ? "Не определен" : mobileAppError.ErrorSource;
        }
    }
}