using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;

namespace NasladdinPlace.Core.Services.UserBalanceCalculator
{
    public class UserPaymentBalanceCalculator : IUserPaymentBalanceCalculator
    {
        private readonly IUserLatestOperationCheckMaker _userLatestOperationCheckMaker;
        private readonly IPaymentBalanceFactory _paymentBalanceFactory;

        public UserPaymentBalanceCalculator(
            IUserLatestOperationCheckMaker userLatestOperationCheckMaker,
            IPaymentBalanceFactory paymentBalanceFactory)
        {
            if (userLatestOperationCheckMaker == null)
                throw new ArgumentNullException(nameof(userLatestOperationCheckMaker));
            if (paymentBalanceFactory == null)
                throw new ArgumentNullException(nameof(paymentBalanceFactory));

            _userLatestOperationCheckMaker = userLatestOperationCheckMaker;
            _paymentBalanceFactory = paymentBalanceFactory;
        }

        public async Task<PaymentBalance> CalculateForUserAsync(int userId)
        {
            var checkResult = await _userLatestOperationCheckMaker.MakeForUserIfOperationUnpaidAsync(userId);

            if (checkResult.Status != UserOperationCheckMakerStatus.Success)
                return PaymentBalance.ZeroOfUser(userId);

            return _paymentBalanceFactory.Create(checkResult.Check, userId);
        }
        
    }
}