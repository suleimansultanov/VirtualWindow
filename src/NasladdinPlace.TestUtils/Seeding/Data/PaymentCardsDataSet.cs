using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class PaymentCardsDataSet : DataSet<PaymentCard>
    {
        private readonly int _userId;

        public PaymentCardsDataSet(int userId)
        {
            _userId = userId;
        }

        protected override PaymentCard[] Data =>
            new[]
            {
                CreateAbleToPayPaymentCard()
            };

        private PaymentCard CreateAbleToPayPaymentCard()
        {
            var paymentCard = new PaymentCard(
                _userId,
                new ExtendedPaymentCardInfo(
                    new PaymentCardNumber("424242", "4242"),
                    DateTime.UtcNow,
                    "477BBA133C182267FE5F086924ABDC5DB71F77BFC27F01F2843F2CDC69D89F05"
                )
            );
            
            paymentCard.MarkAsAbleToMakePayment();

            return paymentCard;
        }
    }
}