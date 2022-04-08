using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class Amounts
    {
        public decimal Electronic { get; }
        public decimal AdvancePayment { get; }
        public decimal Credit { get; }
        public decimal Provision { get; }

        public static Amounts CreateForElectronic(decimal amount)
        {
            return new Amounts(amount, AmountType.Electronic);
        }

        private Amounts(decimal amount, AmountType amountType)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "amount must be greater than zero.");
            if (!Enum.IsDefined(typeof(AmountType), amountType))
                throw new ArgumentException($"Incorrect value {amountType} of the {nameof(AmountType)} enum.");

            switch (amountType)
            {
                case AmountType.Electronic:
                    Electronic = amount;
                    break;
                case AmountType.AdvancePayment:
                    AdvancePayment = amount;
                    break;
                case AmountType.Credit:
                    Credit = amount;
                    break;
                case AmountType.Provision:
                    Provision = amount;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(amountType), amountType, null);
            }
        }
    }
}
