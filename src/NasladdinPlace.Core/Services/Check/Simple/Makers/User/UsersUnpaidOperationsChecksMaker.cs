using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User
{
    public class UsersUnpaidOperationsChecksMaker: IUsersUnpaidOperationsChecksMaker
    {
        private readonly ISimpleCheckMaker _simpleCheckMaker;

        public UsersUnpaidOperationsChecksMaker(
            ISimpleCheckMaker simpleCheckMaker)
        {
            if (simpleCheckMaker == null)
                throw new ArgumentNullException(nameof(simpleCheckMaker));

            _simpleCheckMaker = simpleCheckMaker;
        }

        public async Task<IReadOnlyCollection<PosOperationWithCheck>> MakeForUserAsync(IUnitOfWork unitOfWork, int userId)
        {
            var unpaidPosOperations =
                await unitOfWork.PosOperations.GetAllCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(userId);

            var userPosOperationsWithChecks = new List<PosOperationWithCheck>();

            unpaidPosOperations.ForEach(upo =>
            {
                var simpleCheck = _simpleCheckMaker.MakeCheck(upo);
                userPosOperationsWithChecks.Add(new PosOperationWithCheck(upo, simpleCheck));
            });

            return userPosOperationsWithChecks;
        }
    }
}
