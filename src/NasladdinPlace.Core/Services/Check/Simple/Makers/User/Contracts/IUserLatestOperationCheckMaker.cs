using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts
{
    public interface IUserLatestOperationCheckMaker
    {
        Task<UserLatestOperationCheckMakerResult> MakeForUserIfOperationUnpaidAsync(int userId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserIfOperationUnpaidAsync(IUnitOfWork unitOfWork, int userId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserIfFirstOperationUnpaidAsync(int userId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserIfFirstOperationUnpaidAsync(IUnitOfWork unitOfWork, int userId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserByUnpaidOperationAsync(int userId, int posOperationId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserByUnpaidOperationAsync(IUnitOfWork unitOfWork, int userId, int posOperationId);
        Task<UserLatestOperationCheckMakerResult> MakeForUserAsync(int userId);
    }
}