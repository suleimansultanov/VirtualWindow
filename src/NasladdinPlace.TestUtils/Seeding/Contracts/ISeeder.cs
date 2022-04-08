using System.Collections.Generic;

namespace NasladdinPlace.TestUtils.Seeding.Contracts
{
    public interface ISeeder
    {
        IEnumerable<T> Seed<T>(IEnumerable<T> seeds) where T: class;
    }
}