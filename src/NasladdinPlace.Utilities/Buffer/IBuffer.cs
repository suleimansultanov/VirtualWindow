using System.Collections.Generic;

namespace NasladdinPlace.Utilities.Buffer
{
    public interface IBuffer<TEntity>
    {
        void Add(TEntity element);
        IEnumerable<TEntity> GetAll();
        void Clear();
    }
}
