using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.Printers
{
    public class DeviceInfoMessageRussianPrinter : BaseMessagePrinter<DeviceInfo>
    {
        protected override IEnumerable<string> ProvideMessagePieces(DeviceInfo entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            var deviceInfo = entity;
            return new[]
            {
                $"Модель телефона: {deviceInfo.DeviceName}",
                $"Операционная система: {deviceInfo.OperatingSystem}"
            };
        }
    }
}