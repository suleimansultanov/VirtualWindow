using System;
using System.Globalization;
using CloudPaymentsClient.Rest.Dtos.Payment;
using NasladdinPlace.Payment.Models;

namespace CloudPaymentsClient.Rest.Mappers
{
    public class PaymentResponseToPaymentCardInfoMapper : IPaymentResponseToPaymentCardInfoMapper
    {
        private const string MonthAndYearSeparatedBySlashDateTimeFormat = "MM/y";
        
        public PaymentCardInfo Transform(PaymentResponseDto paymentResponseDto)
        {
            if (paymentResponseDto == null)
                throw new ArgumentNullException(nameof(paymentResponseDto));

            if (!TryParseMonthAndYearSeparatedBySlash(paymentResponseDto.CardExpDate, out var cardExpirationDate))
                throw new ArgumentException(
                    $"Card expiration date is not in a correct format: {paymentResponseDto.CardExpDate}"
                );

            var paymentCardNumber = new PaymentCardNumber(
                paymentResponseDto.CardFirstSix,
                paymentResponseDto.CardLastFour
            );
            var cardToken = paymentResponseDto.CardToken;
            return new PaymentCardInfo(paymentCardNumber, cardExpirationDate, cardToken);
        }

        private static bool TryParseMonthAndYearSeparatedBySlash(
            string monthAndYearSeparatedBySlash, out DateTime dateTime)
        {
            return DateTime.TryParseExact(
                monthAndYearSeparatedBySlash,
                MonthAndYearSeparatedBySlashDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime
            );
        }
    }
}