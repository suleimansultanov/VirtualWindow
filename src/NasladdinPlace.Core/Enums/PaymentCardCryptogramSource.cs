using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PaymentCardCryptogramSource
    {
        [EnumDescription("Банковская карта")]
        Common = 0,

        [EnumDescription("Apple Pay")]
        ApplePay = 1,

        [EnumDescription("Google Pay")]
        GooglePay = 2
    }
}