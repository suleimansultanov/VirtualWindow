using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckUserInfo
    {
        public int UserId { get; }
        public string UserName { get; }
        public decimal TotalBonusAmount { get; }

        public DetailedCheckUserInfo(int userId, string userName, decimal totalBonusAmount)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            if (totalBonusAmount < 0)
                throw new ArgumentException($"User's bonus amount must be greater or equal to zero. " +
                                            $"But found: {totalBonusAmount}.");
            
            UserId = userId;
            UserName = userName;
            TotalBonusAmount = totalBonusAmount;
        }

        public DetailedCheckUserInfo(ApplicationUser user)
            : this(user.Id, user.UserName, user.TotalBonusPoints)
        {
        }
    }
}