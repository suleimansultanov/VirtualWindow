using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using NasladdinPlace.Logging;
using System;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Helpers
{
    public class PaymentInfoCreator : IPaymentInfoCreator
    {
        private readonly ILogger _logger;

        public PaymentInfoCreator(ILogger logger)
        {
            _logger = logger;
        }

        public PaymentInfo Create(bool isNewPaymentSystem, PosOperationTransaction posOperationTransaction, SimpleCheck simpleCheck, PaymentCard paymentCard)
        {
            if (simpleCheck == null)
                throw new ArgumentNullException(nameof(simpleCheck));

            if (isNewPaymentSystem)
            {
                if (posOperationTransaction != null)
                {
                    var isFreeOrEmpty = posOperationTransaction.MoneyAmount == decimal.Zero;

                    return new PaymentInfo(
                        posOperationTransaction.PosOperationId,
                        isFreeOrEmpty,
                        posOperationTransaction.GetTotalCostWithDiscount(),
                        posOperationTransaction.BonusAmount,
                        paymentCard);
                }

                _logger.LogError($"An error occurred while trying paid for purchase by new payment system. PosOperation {simpleCheck.Id} doesn't have a posOperationTransaction. Take the amount from PosOperation");
            }

            return new PaymentInfo(
                simpleCheck.Id,
                simpleCheck.IsFreeOrEmpty,
                simpleCheck.Summary.CostSummary.CostWithDiscount,
                simpleCheck.Summary.BonusInfo.WrittenOffBonusAmount,
                paymentCard);
        }
    }
}
