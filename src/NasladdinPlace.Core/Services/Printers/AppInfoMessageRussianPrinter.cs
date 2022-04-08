using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.Printers
{
    public class AppInfoMessageRussianPrinter : BaseMessagePrinter<AppInfo>
    {
        protected override IEnumerable<string> ProvideMessagePieces(AppInfo entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            var appInfo = entity;
            return new[]
            {
                $"Версия приложения: {appInfo.AppVersion}"
            };
        }
    }
}