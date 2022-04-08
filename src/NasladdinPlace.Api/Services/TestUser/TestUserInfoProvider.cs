using System;
using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Core.Services.Users.Test.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Services.TestUser
{
    public class TestUserInfoProvider : ITestUserInfoProvider
    {
        private readonly TestUserInfo _testUserInfo;

        public TestUserInfoProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            string userName,
            bool isPaymentCardVerificationRequired)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            _testUserInfo = CreateTestUserInfo(
                unitOfWorkFactory,
                userName,
                isPaymentCardVerificationRequired
            ).Result;
        }

        public Task<bool> IsTestUserAsync(int userId)
        {
            return Task.FromResult(_testUserInfo?.DoesBelongToUser(userId) ?? false);
        }

        public bool IsTestUser(ApplicationUser user)
        {
            return _testUserInfo.DoesBelongToUser(user);
        }

        public Task<ValueResult<IReadOnlyTestUserInfo>> ProvideTestUserInfoAsync(int userId)
        {
            var testUserInfoValueResult = _testUserInfo?.UserId != userId
                ? ValueResult<IReadOnlyTestUserInfo>.Failure() 
                : ValueResult<IReadOnlyTestUserInfo>.Success(_testUserInfo);

            return Task.FromResult(testUserInfoValueResult);
        }
        
        private static async Task<TestUserInfo> CreateTestUserInfo(
            IUnitOfWorkFactory unitOfWorkFactory,
            string userName, 
            bool isPaymentCardVerificationRequired)
        {
            using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = await unitOfWork.Users.GetByNameAsync(userName);
                if (user == null) return null;
                
                return new TestUserInfo(user.Id, userName)
                {
                    IsPaymentCardVerificationRequired = isPaymentCardVerificationRequired
                };
            }
        }
    }
}