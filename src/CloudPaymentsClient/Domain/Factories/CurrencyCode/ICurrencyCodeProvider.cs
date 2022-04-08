using NasladdinPlace.Payment.Models;

namespace CloudPaymentsClient.Domain.Factories.CurrencyCode
{
    public interface ICurrencyCodeProvider
    {
        string Provide(Currency currency);
    }
}