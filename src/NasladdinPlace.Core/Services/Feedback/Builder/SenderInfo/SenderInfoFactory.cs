using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using NasladdinPlace.Core.Services.UserBalanceCalculator;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo
{
    public class SenderInfoFactory : ISenderInfoFactory
    {
        private readonly IUserLatestOperationCheckMaker _userLatestOperationCheckMaker;
        private readonly IPaymentBalanceFactory _paymentBalanceFactory;

        public SenderInfoFactory(IUserLatestOperationCheckMaker userLatestOperationCheckMaker,
            IPaymentBalanceFactory paymentBalanceFactory)
        {
            if (userLatestOperationCheckMaker == null)
                throw new ArgumentNullException(nameof(userLatestOperationCheckMaker));
            if (paymentBalanceFactory == null)
                throw new ArgumentNullException(nameof(paymentBalanceFactory));

            _userLatestOperationCheckMaker = userLatestOperationCheckMaker;
            _paymentBalanceFactory = paymentBalanceFactory;
        }

        public async Task<Models.Feedback.SenderInfo> CreateAsync(SenderCreationInfo info)
        {
            if (info.IsSenderUnauthorized)
            {
                return Models.Feedback.SenderInfo.UnauthorizedSenderInfo(info.PhoneNumber, info.DeviceInfo);
            }

            var userId = info.User.Id;
            var checkResult = await _userLatestOperationCheckMaker.MakeForUserAsync(userId);

            if(checkResult.Status == UserOperationCheckMakerStatus.Success)
                return CreateSenderInfoForSuccessPosOperation(info, checkResult);

            return Models.Feedback.SenderInfo.AuthorizedSenderInfoWithoutLastPurchase(info.User, info.DeviceInfo, PaymentBalance.ZeroOfUser(userId));
        }

        private Models.Feedback.SenderInfo CreateSenderInfoForSuccessPosOperation(SenderCreationInfo info,
            UserLatestOperationCheckMakerResult checkResult)
        {
            var userId = info.User.Id;
            var check = checkResult.Check;

            var lastPurchaseInfo = new LastPurchaseInfo(checkResult.CheckPosOperation, check);
            var paymentBalance = _paymentBalanceFactory.Create(check, userId);

            return Models.Feedback.SenderInfo.AuthorizedSenderInfoWithLastPurchase(info.User, info.DeviceInfo,
                lastPurchaseInfo, paymentBalance);
        }
    }
}
