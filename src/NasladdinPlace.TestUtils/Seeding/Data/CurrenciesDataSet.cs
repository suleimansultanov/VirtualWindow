using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class CurrenciesDataSet : DataSet<Currency>
    {
        protected override Currency[] Data => new[]
        {
            new Currency(0,"руб", "RUB")
        };
    }
}