using System;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckBonusInfo
    {
        public static readonly SimpleCheckBonusInfo Empty =
            new SimpleCheckBonusInfo(decimal.Zero, decimal.Zero);
        
        public decimal WrittenOffBonusAmount { get; }
        public decimal AccruedBonusAmount { get; }
        
        public SimpleCheckBonusInfo(
            decimal writtenOffBonusAmount,
            decimal accruedBonusAmount)
        {
            if (writtenOffBonusAmount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(writtenOffBonusAmount), 
                    writtenOffBonusAmount,
                    $"Written off bonus amount must be greater or equal to zero. But found {writtenOffBonusAmount}."
                );
            if (accruedBonusAmount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(accruedBonusAmount),
                    accruedBonusAmount,
                    $"Accrued bonus amount must be greater or equal to zero. But found {accruedBonusAmount}."
                );

            WrittenOffBonusAmount = writtenOffBonusAmount;
            AccruedBonusAmount = accruedBonusAmount;
        }
    }
}