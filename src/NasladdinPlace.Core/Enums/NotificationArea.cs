using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    /// <summary>
    /// Area where the notification came from
    /// </summary>
    public enum NotificationArea : byte
    {
        /// <summary>
        /// User registration
        /// </summary>
        [EnumDescription("Регистрация")]
        Registration = 1,

        /// <summary>
        /// At the time of purchase
        /// </summary>
        [EnumDescription("Покупка")]
        Purchase = 2,

        /// <summary>
        /// Other message area
        /// </summary>
        [EnumDescription("Другое")]
        Other = 3,

        /// <summary>
        /// Promotion for verify phone number
        /// </summary>
        [EnumDescription("Промо акция: Подтверждение регистрации")]
        PromotionVerifyPhoneNumber = 4,

        /// <summary>
        /// Promotion for verify phone number
        /// </summary>
        [EnumDescription("Промо акция: Привязка карты")]
        PromotionVerifyPaymentCard = 5,

        /// <summary>
        /// Promotion for verify phone number
        /// </summary>
        [EnumDescription("Промо акция: Первая покупка")]
        PromotionFirstPay = 6,

        /// <summary>
        /// Link to fiscalization info
        /// </summary>
        [EnumDescription("Информация о фискализации")]
        Fiscalization = 7,

        /// <summary>
        /// Refund
        /// </summary>
        [EnumDescription("Возврат средств")]
        Refund = 8,
        /// <summary>
        /// Addition or Verification
        /// </summary>
        [EnumDescription("Списание средств")]
        AdditionOrVerification = 9
    }
}
