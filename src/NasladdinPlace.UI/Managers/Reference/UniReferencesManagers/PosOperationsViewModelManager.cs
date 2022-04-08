using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.ViewModels.Base;
using NasladdinPlace.UI.ViewModels.Checks;
using PagedList.Interfaces;
using System.Linq;
using AutoMapper;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Builders;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers
{
    public class PosOperationsViewModelManager : BaseUniReferencesManager
    {
        public PosOperationsViewModelManager(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ReferenceDataWithPagination GetPosOperationViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var queryBuilder = new PosOperationsViewModelQueryBuilder(parameters, UnitOfWork);

            var query = queryBuilder
                .ApplyTotalPriceContextFilter()
                .ApplyStatusContextFilter()
                .ApplyOperationModeFilter()
                .ApplyFromToDateFilterByStatus()
                .ApplyUnverifiedChekItemsFilter()
                .ApplyPaymentCardCryptogramSourceFilter()
                .ApplyFiscalizationInfoErrorsFilter()
                .Build();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel) Mapper.Map<PosOperationViewModel>(e))
                .ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }
    }
}
