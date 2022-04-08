using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Test.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Users.Test
{
    public interface ITestUserInfoProvider
    {
        Task<bool> IsTestUserAsync(int userId);
        bool IsTestUser(ApplicationUser user);
        Task<ValueResult<IReadOnlyTestUserInfo>> ProvideTestUserInfoAsync(int userId);
    }
}