using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public abstract class DataSet<T> : IEnumerable<T> where T: class
    {
        public IEnumerator<T> GetEnumerator()
        {
            return Data.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract T[] Data { get; }
    }
}