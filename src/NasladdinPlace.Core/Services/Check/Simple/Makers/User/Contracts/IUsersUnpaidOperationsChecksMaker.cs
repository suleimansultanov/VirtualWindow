using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts
{
    public interface IUsersUnpaidOperationsChecksMaker
    {
        Task<IReadOnlyCollection<PosOperationWithCheck>> MakeForUserAsync(IUnitOfWork unitOfWork, int userId);
    }
}
