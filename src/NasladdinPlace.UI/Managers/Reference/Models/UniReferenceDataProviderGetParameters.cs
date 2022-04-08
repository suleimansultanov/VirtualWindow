using System.Collections.Generic;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Reference;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class UniReferenceDataProviderGetParameters
    {
        public List<BaseFilterItemModel> Filter { get; }
        public PaginationInfo Pagination { get; }
        public IUnitOfWork UnitOfWork { get; }

        public UniReferenceDataProviderGetParameters(List<BaseFilterItemModel> filter, PaginationInfo pagination, IUnitOfWork unitOfWork)
        {
            Filter = filter;
            Pagination = pagination;
            UnitOfWork = unitOfWork;
        }
    }
}