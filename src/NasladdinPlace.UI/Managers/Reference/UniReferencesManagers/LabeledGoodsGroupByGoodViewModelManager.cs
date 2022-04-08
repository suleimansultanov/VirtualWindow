using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.ViewModels.Base;
using NasladdinPlace.UI.ViewModels.LabeledGoods;
using PagedList.Interfaces;
using System;
using System.Linq;
using NasladdinPlace.Core.Services.OverdueGoods.Extensions;
using AutoMapper;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers
{
    public class LabeledGoodsGroupByGoodViewModelManager : BaseUniReferencesManager
    {
        public LabeledGoodsGroupByGoodViewModelManager(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ReferenceDataWithPagination GetLabeledGoodsGroupByGoodViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            var hasOverdueType =
                parameters.Filter.GetValue(nameof(OverdueGoodsFilterContext.Type));
            var overdueType = int.TryParse(hasOverdueType, out var overdueTypeAsInt) ? (OverdueType) overdueTypeAsInt: (OverdueType?) null;

            var query = GetLabeledGoodsByOverdueType(overdueType).Select(lg =>
                Mapper.Map<LabeledGood, LabeledGoodsGroupByGoodViewModelForFilter>(lg));

            var baseQuery = query.DynamicFilter(parameters.Filter);       
            var posId = parameters.Filter.GetValueInt(nameof(OverdueGoodsFilterContext.PosId));

            var customData = new
            {
                Count = baseQuery.Count(),
                PosId = posId ?? 0
            };

            var baseGroupedQuery = baseQuery.GroupBy(group => new { group.GoodId, group.Price });
            baseGroupedQuery = baseGroupedQuery.Pagination(parameters.Pagination);

            var baseList = baseGroupedQuery.AsEnumerable()
                .Select(e => (BaseViewModel) new LabeledGoodsGroupByGoodViewModel
                {
                    GoodId = e.Key.GoodId,
                    Price = e.Key.Price,
                    GoodName = e.FirstOrDefault()?.Name,
                    LabeledGoodsCount = e.Count(),
                    LabeledGoods = e.Select(lg => new LabeledGoodBasicInfoViewModel
                    {
                        Id = lg.Id,
                        IsDisabled = lg.IsDisabled,
                        Label = lg.Label
                    })
                }).ToList();

            var queryAsPaginationInfo = baseGroupedQuery.AsQueryable() as IPaginationInfo;

            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination
                {Data = baseList, PaginationInfo = paginationInfo, CustomData = customData};
        }

        private IQueryable<LabeledGood> GetLabeledGoodsByOverdueType(OverdueType? overdueType)
        {
            var timeSpanByOverdueType = new TimeSpanRange(TimeSpan.MinValue, TimeSpan.MaxValue);

            if (overdueType.HasValue)
            {
                timeSpanByOverdueType = ((OverdueType)overdueType).GetTimeSpanRange();
            }

            var utcDateTimeRange = timeSpanByOverdueType.CreateDateTimeRange(DateTime.UtcNow);

            return UnitOfWork.LabeledGoods.GetOverdueForDateTimeRange(utcDateTimeRange);
        }
    }
}
