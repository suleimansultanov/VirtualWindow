using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.Printers.Common;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Printers
{
    public class UserShortInfoRussianPrinter : BaseMessagePrinter<UserShortInfo>
    {
        protected override IEnumerable<string> ProvideMessagePieces(UserShortInfo entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            var userShortInfo = entity;
            return new[]
            {
                $"Номер телефона: {userShortInfo.UserName}"
            };
        }
    }
}