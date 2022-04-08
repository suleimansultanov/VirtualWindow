using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Core.Services.Payment.Card.Type
{
    public static class PaymentCardTypeByNumberProvider
    {
        private static readonly IDictionary<PaymentCardType, ISet<string>> RequiredPrefixesByPaymentCardType = new Dictionary<PaymentCardType, ISet<string>>
        {
            { PaymentCardType.Visa, new SortedSet<string> { "4" } },
            { PaymentCardType.Mastercard, new SortedSet<string> { "51", "52", "53", "54", "55" } }
        };

        public static PaymentCardType Provide(PaymentCardNumber number)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));

            var firstDigits = number.FirstSixDigits;

            foreach (var (paymentCardType, prefixes) in RequiredPrefixesByPaymentCardType)
            {
                if (prefixes.Any(p => firstDigits.StartsWith(p))) return paymentCardType;
            }

            return PaymentCardType.Unknown;
        }
    }
}