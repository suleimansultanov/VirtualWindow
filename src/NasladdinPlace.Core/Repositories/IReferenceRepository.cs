using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;

namespace NasladdinPlace.Core.Repositories
{
    public interface IReferenceRepository : IDisposable
    {
        IEnumerable<Entity> GetAll(Type type);

        IQueryable<Entity> Get(Type type, List<BaseFilterItemModel> filter = null, PaginationInfo pagination = null);
    }
}
