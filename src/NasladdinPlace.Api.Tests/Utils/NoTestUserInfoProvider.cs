using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Core.Services.Users.Test.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Tests.Utils
{
    public class NoTestUserInfoProvider : ITestUserInfoProvider
    {
        public Task<bool> IsTestUserAsync(int userId)
        {
            return Task.FromResult(false);
        }

        public bool IsTestUser(ApplicationUser user)
        {
            return false;
        }

        public Task<ValueResult<IReadOnlyTestUserInfo>> ProvideTestUserInfoAsync(int userId)
        {
            return Task.FromResult(ValueResult<IReadOnlyTestUserInfo>.Failure());
        }
    }
}