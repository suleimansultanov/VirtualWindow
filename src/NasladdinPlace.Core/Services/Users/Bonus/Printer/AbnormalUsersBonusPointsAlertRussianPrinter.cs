using System.Collections.Generic;
using System.Text;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Bonus.Printer.Contracts;

namespace NasladdinPlace.Core.Services.Users.Bonus.Printer
{
    public class AbnormalUsersBonusPointsAlertRussianPrinter : IAbnormalUsersBonusPointsAlertPrinter
    {
        public string Print(IEnumerable<ApplicationUser> users)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"{Emoji.Purse} Неправильное количество бонусов у пользователей:");

            var index = 0;

            foreach (var user in users)
            {
                messageBuilder.AppendLine($"{++index}. Пользователь {user.UserName} — {user.TotalBonusPoints:f2}");
            }

            return messageBuilder.ToString();
        }
    }
}