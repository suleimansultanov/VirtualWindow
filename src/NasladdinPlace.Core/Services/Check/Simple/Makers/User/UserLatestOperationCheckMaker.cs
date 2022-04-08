using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User
{
    public class UserLatestOperationCheckMaker : IUserLatestOperationCheckMaker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISimpleCheckMaker _simpleCheckMaker;

        public UserLatestOperationCheckMaker(
            IUnitOfWorkFactory unitOfWorkFactory,
            ISimpleCheckMaker simpleCheckMaker)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (simpleCheckMaker == null)
                throw new ArgumentNullException(nameof(simpleCheckMaker));

            _unitOfWorkFactory = unitOfWorkFactory;
            _simpleCheckMaker = simpleCheckMaker;
        }

        public async Task<UserLatestOperationCheckMakerResult> MakeForUserIfOperationUnpaidAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await MakeForUserIfOperationUnpaidAsync(unitOfWork, userId);
            }
        }

        public Task<UserLatestOperationCheckMakerResult> MakeForUserIfOperationUnpaidAsync(IUnitOfWork unitOfWork, int userId)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return MakeCheckResultAsync(userId, unitOfWork.PosOperations.GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync);
        }

        public async Task<UserLatestOperationCheckMakerResult> MakeForUserIfFirstOperationUnpaidAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await MakeForUserIfFirstOperationUnpaidAsync(unitOfWork, userId);
            }
        }

        public Task<UserLatestOperationCheckMakerResult> MakeForUserIfFirstOperationUnpaidAsync(IUnitOfWork unitOfWork, int userId)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return MakeCheckResultAsync(userId, unitOfWork.PosOperations.GetFirstCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync);
        }

        public async Task<UserLatestOperationCheckMakerResult> MakeForUserAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await MakeCheckResultAsync(userId, unitOfWork.PosOperations.GetUserLatestIncludingCheckItemsAsync);
            }
        }

        public Task<UserLatestOperationCheckMakerResult> MakeForUserByUnpaidOperationAsync(IUnitOfWork unitOfWork, int userId, int posOperationId)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            return MakeCheckResultAsync(unitOfWork, userId, posOperationId);
        }

        public async Task<UserLatestOperationCheckMakerResult> MakeForUserByUnpaidOperationAsync(int userId, int posOperationId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await MakeCheckResultAsync(unitOfWork, userId, posOperationId);
            }
        }

        private async Task<UserLatestOperationCheckMakerResult> MakeCheckResultAsync(int userId, Func<int, Task<PosOperation>> getPosOperationAsync)
        {
            var latestUserOperation = await getPosOperationAsync(userId);

            if (latestUserOperation == null)
                return UserLatestOperationCheckMakerResult.PosOperationNotFound();

            var check = _simpleCheckMaker.MakeCheck(latestUserOperation);

            return UserLatestOperationCheckMakerResult.Success(check, latestUserOperation);
        }

        private async Task<UserLatestOperationCheckMakerResult> MakeCheckResultAsync(IUnitOfWork unitOfWork, int userId, int posOperationId)
        {
            var latestUserOperation = await unitOfWork.PosOperations.GetUnpaidCheckItemOfUserByPosOperationIdAsync(userId, posOperationId);

            if (latestUserOperation == null)
                return UserLatestOperationCheckMakerResult.PosOperationNotFound();


            var check = _simpleCheckMaker.MakeCheck(latestUserOperation);

            return UserLatestOperationCheckMakerResult.Success(check, latestUserOperation);
        }
    }
}
