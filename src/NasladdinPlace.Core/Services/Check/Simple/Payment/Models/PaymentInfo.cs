using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Models
{
    public class PaymentInfo
    {
        public int PosOperationId { get; }
        public bool IsFreeOrEmpty { get; }
        public decimal CostWithDiscount { get; }
        public decimal WrittenOffBonusAmount { get; }
        public PaymentCard PaymentCard { get; }

        public PaymentInfo(int posOperationId, bool isFreeOrEmpty, decimal costWithDiscount, decimal writtenOffBonusAmount, PaymentCard paymentCard)
        {
            PosOperationId = posOperationId;
            IsFreeOrEmpty = isFreeOrEmpty;
            CostWithDiscount = costWithDiscount;
            WrittenOffBonusAmount = writtenOffBonusAmount;
            PaymentCard = paymentCard;
        }
    }
}
