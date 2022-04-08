using AutoMapper;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.ViewModels.Base;
using NasladdinPlace.UI.ViewModels.Users;
using PagedList.Interfaces;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.UI.Managers.Reference.Models;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers
{
    public class UsersViewModelManager : BaseUniReferencesManager
    {
        public UsersViewModelManager(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ReferenceDataWithPagination GetUsersViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var query = UnitOfWork.Users.GetAllIncludingPosOperationAndUserRoles();

            var filterContextStatus = parameters.Filter.GetItem(x => x.FilterName == nameof(UserLazinessIndex.Lazy));
            if (filterContextStatus != null)
                query = query.Where(ap => ap.PosOperations.Any());

            var mappingQueryList = query.Select(e => Mapper.Map<UserViewModel>(e)).ToImmutableList();

            var baseQuery = mappingQueryList
                .AsQueryable()
                .DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)e).ToList();
            
            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }
    }
}
