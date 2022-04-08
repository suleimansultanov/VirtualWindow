using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class CountriesDataSet : DataSet<Country>
    {
        protected override Country[] Data => new[]
        {
            new Country(0, "Россия")
        };
    }
}