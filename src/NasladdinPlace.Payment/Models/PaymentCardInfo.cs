using System;

namespace NasladdinPlace.Payment.Models
{
    public class PaymentCardInfo
    {
        public PaymentCardNumber Number { get; }
        public DateTime ExpirationDate { get; }
        public string Token { get; }

        public PaymentCardInfo(PaymentCardNumber number, DateTime expirationDate, string token)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            Number = number;
            ExpirationDate = expirationDate;
            Token = token;
        }
    }
}