using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class MakersDataSet : DataSet<Maker>
    {
        protected override Maker[] Data => new[]
        {
            new Maker(0, "Тестовый производитель")
        };
}
}