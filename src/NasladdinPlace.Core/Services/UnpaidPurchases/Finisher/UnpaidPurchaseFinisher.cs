using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.UnpaidPurchases.Finisher
{
    public class UnpaidPurchaseFinisher : IUnpaidPurchaseFinisher
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPurchaseManager _purchaseManager;
        private readonly ILogger _logger;

        public UnpaidPurchaseFinisher(IUnitOfWorkFactory unitOfWorkFactory, IPurchaseManager purchaseManager, ILogger logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _purchaseManager = purchaseManager;
            _logger = logger;
        }

        public async Task FinishUnpaidPurchasesAsync(TimeSpan considerUnpaidAfter)
        {
            try
            {
                _logger.LogInfo("Start method FinishUnpaidPurchases");
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var debtors = await unitOfWork.Users.GetDebtorsAbleToPayAsync(considerUnpaidAfter);
                    var debtorsId = debtors.Select(u => u.Id).ToList();
                    await _purchaseManager.CompletionManager.CompletePurchasesOfUsersAsync(debtorsId);
                }
                _logger.LogInfo("The method FinishUnpaidPurchases has been successefully finished");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing debtors unpaid purchases. Error: {ex}");
            }
        }
    }
}