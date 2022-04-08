using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Test.Models
{
    public class TestUserInfo : IReadOnlyTestUserInfo
    {
        public int UserId { get; }
        public string UserName { get; }
        public bool IsPaymentCardVerificationRequired { get; set; }

        public TestUserInfo(int userId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            UserId = userId;
            UserName = userName;
        }

        public bool DoesBelongToUser(ApplicationUser user)
        {
            return user?.UserName == UserName;
        }

        public bool DoesBelongToUser(int userId)
        {
            return userId == UserId;
        }
    }
}