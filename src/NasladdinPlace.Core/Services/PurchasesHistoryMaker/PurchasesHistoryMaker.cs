using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker.Models;

namespace NasladdinPlace.Core.Services.PurchasesHistoryMaker
{
    public class PurchasesHistoryMaker : IPurchasesHistoryMaker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISimpleCheckMaker _simpleCheckMaker;

        public PurchasesHistoryMaker(
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

        public async Task<PurchaseHistory> MakeNonEmptyChecksForUserAsync(int userId)
        {
            var purchaseHistory = await GetPurchaseHistory(userId, c => !c.Summary.CostSummary.IsEmpty);

            return purchaseHistory;
        }

        public async Task<PurchaseHistory> MakeChecksWithItemsForUserAsync(int userId)
        {
            var purchaseHistory = await GetPurchaseHistory(userId, c => c.Items.Count > 0);

            return purchaseHistory;
        }

        private async Task<PurchaseHistory> GetPurchaseHistory(int userId, Func<SimpleCheck, bool> predicate)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posOperations =
                    await unitOfWork.PosOperations.GetByUserIncludingCheckItemsAndFiscalizationInfoOrderedByDateStartedAsync(userId);

                var checks = _simpleCheckMaker.MakeChecks(posOperations).ToImmutableList();

                var filteredChecks = checks.Where(predicate).ToImmutableList();

                return new PurchaseHistory(filteredChecks);
            }
        }
    }
}