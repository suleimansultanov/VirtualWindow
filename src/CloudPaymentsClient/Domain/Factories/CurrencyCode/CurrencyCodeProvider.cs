using System;
using NasladdinPlace.Payment.Models;

namespace CloudPaymentsClient.Domain.Factories.CurrencyCode
{
    public class CurrencyCodeProvider : ICurrencyCodeProvider
    {
        public string Provide(Currency currency)
        {
            switch (currency)
            {
                case Currency.Rubles:
                    return "RUB";
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(currency),
                        currency,
                        $"Unable to find the specified {nameof(Currency)} in the system."
                     );
            }
        }
    }
}