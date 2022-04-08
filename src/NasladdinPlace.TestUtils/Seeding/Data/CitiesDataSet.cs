using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class CitiesDataSet : DataSet<City>
    {
        protected override City[] Data => new[]
        {
            new City(0, "Москва", 1)
        };
    }
}