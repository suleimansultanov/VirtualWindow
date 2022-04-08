using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class PromotionSettingsDataSet : DataSet<PromotionSetting>
    {
        public const decimal VerifyPhoneNumberBonusAmount = 50M;
        public const decimal FirstPayBonusAmount = 50M;

        protected override PromotionSetting[] Data => new[]
        {
            new PromotionSetting(PromotionType.VerifyPhoneNumber, VerifyPhoneNumberBonusAmount, true, true,
                TimeSpan.FromHours(10)),
            new PromotionSetting(PromotionType.FirstPay, FirstPayBonusAmount, true, true, TimeSpan.FromHours(10))
        };
    }
}