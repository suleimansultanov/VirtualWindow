using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Bonus.Printer.Contracts
{
    public interface IAbnormalUsersBonusPointsAlertPrinter
    {
        string Print(IEnumerable<ApplicationUser> users);
    }
}