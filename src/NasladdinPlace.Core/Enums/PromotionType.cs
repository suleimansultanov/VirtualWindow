using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    /// <summary>
    /// Promotion types
    /// </summary>
    public enum PromotionType : byte
    {
        /// <summary>
        /// Promotion for phone number confirmation
        /// </summary>
        [EnumDescription("Подтверждение регистрации")]
        VerifyPhoneNumber,

        /// <summary>
        /// Promotion for credit card confirmation
        /// </summary>
        [EnumDescription("Привязка карты")]
        VerifyPaymentCard,

        /// <summary>
        /// Promotion for first pay
        /// </summary>
        [EnumDescription("Первая покупка")]
        FirstPay
    }
}
