using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.Managers.Reference.UniReferencesManagers;
using NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Models;
using NasladdinPlace.UI.ViewModels.Base;
using NasladdinPlace.UI.ViewModels.Discounts;
using NasladdinPlace.UI.ViewModels.Goods;
using NasladdinPlace.UI.ViewModels.LabeledGoods;
using NasladdinPlace.UI.ViewModels.Logs;
using NasladdinPlace.UI.ViewModels.Media;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.UI.ViewModels.PosScreenTemplates;
using NasladdinPlace.UI.ViewModels.PosSensor;
using NasladdinPlace.UI.ViewModels.Roles;
using NasladdinPlace.UI.ViewModels.Schedules;
using NasladdinPlace.UI.ViewModels.Security;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.EnumHelpers;
using PagedList.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PosTemperatureDto = NasladdinPlace.Dtos.Pos.PosTemperatureDto;

namespace NasladdinPlace.UI.Managers.Reference
{
    public partial class ReferenceManager
    {
        private ReferenceDataWithPagination GetScheduleViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var schedulesResult = Task.Run(() => _nasladdinApiClient.GetSchedulesAsync()).Result;
            if (!schedulesResult.IsRequestSuccessful)
                return new ReferenceDataWithPagination
                {
                    PaginationInfo = parameters.Pagination,
                    Data = new List<BaseViewModel>()
                };

            var query = schedulesResult.Result.AsQueryable();

            var baseQuery = query.Select(e => Mapper.Map<ScheduleViewModel>(e));
            baseQuery = baseQuery.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)e).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetPosMediaContentViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.PosMediaContents.GetAll().Include(p => p.MediaContent);
            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map(e, typeof(PosMediaContent), typeof(PosMediaContentViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetPosOperationViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            return new PosOperationsViewModelManager(_unitOfWork).GetPosOperationViewModel(parameters);
        }

        private ReferenceDataWithPagination GetPosViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var roleFilterItem = parameters.Filter.GetFilter(nameof(PosViewModel.AssignedRoles));
            var pointsOfSaleResult = new RequestResponse<IEnumerable<PosDto>>();

            pointsOfSaleResult = bool.TryParse(roleFilterItem.Value, out bool isAdmin) && isAdmin
                ? _nasladdinApiClient.GetPointsOfSaleAsync().Result
                : _nasladdinApiClient.GetPointsOfSaleByRoleAsync().Result;

            if (!pointsOfSaleResult.IsRequestSuccessful)
                return new ReferenceDataWithPagination
                {
                    PaginationInfo = parameters.Pagination,
                    Data = new List<BaseViewModel>()
                };

            var query = pointsOfSaleResult.Result.AsQueryable();

            if (parameters.Filter.GetFilter(nameof(PosViewModel.IsNotDeactivated)) == null)
                query = query.Where(bq => bq.IsNotDeactivated.Value);

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map<PosViewModel>(e)).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetMediaContentViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.GetRepository<MediaContent>().GetAll().Select(m => new { m.ContentType, m.FileName, m.UploadDateTime, m.Id });
            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)new MediaContentViewModel
            {
                ContentType = (int)e.ContentType,
                FileName = e.FileName,
                UploadDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(e.UploadDateTime),
                MediaContentId = e.Id
            }).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetUserViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            return new UsersViewModelManager(_unitOfWork).GetUsersViewModel(parameters);
        }

        private ReferenceDataWithPagination GetPosTemperatureDetailsViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var emptyData = new ReferenceDataWithPagination
            {
                PaginationInfo = parameters.Pagination,
                Data = new List<BaseViewModel>()
            };

            var posId = parameters.Filter.GetValueInt(nameof(PosTemperatureDetailsViewModel.PosId));

            if (!posId.HasValue)
                return emptyData;

            var posTemperaturesResult = Task.Run(() => _nasladdinApiClient.GetTemperaturesByPosIdAsync(posId.Value)).Result;

            if (!posTemperaturesResult.IsRequestSuccessful)
                return emptyData;

            var result = posTemperaturesResult.Result.ToList();

            var customData = new
            {
                Count = result.Count,
                AverageTemperature = result.Any() ? result.Average(r => r.Temperature) : 0
            };

            var query = result.AsQueryable();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);
            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map(e, typeof(PosTemperatureDto), typeof(PosTemperatureDetailsViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo, CustomData = customData };
        }

        private ReferenceDataWithPagination GetLabeledGoodTrackingRecordViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            var query =
                _unitOfWork.LabeledGoodTrackingRecords.GetAllIncludingPosAndLabeledGood();

            var goodId = parameters.Filter.GetValueInt(nameof(LabeledGoodTrackingRecordViewModel.GoodId));

            if (goodId.HasValue)
                query = query.Where(q => q.LabeledGood.GoodId == goodId.Value);

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e =>
                (BaseViewModel)Mapper.Map(e, typeof(LabeledGoodTrackingRecord),
                    typeof(LabeledGoodTrackingRecordViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetPosAbnormalSensorMeasurementsViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.PosAbnormalSensorMeasurements.GetAllIncludingPos();

            var posIdFilter = parameters.Filter.GetItem(x => x.FilterName == nameof(PosAbnormalSensorMeasurementViewModel.PosId));

            var posId = posIdFilter == null
                ? null
                : parameters.Filter.GetValueInt(nameof(PosTemperatureDetailsViewModel.PosId));

            if (posId.HasValue)
                query = query.Where(ps => ps.PosId == posId);

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map(e, typeof(PosAbnormalSensorMeasurement), typeof(PosAbnormalSensorMeasurementViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetPosLogsViewModel(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.PosLogs.GetAllIncludePos()
                .Select(pl => Mapper.Map<PosLog, PosLogsViewModelForFilter>(pl));

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map<PosLogsViewModelForFilter, PosLogViewModel>(e))
                .ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetDiscounts(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.Discounts.GetAllIncludePosDiscounts();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map(e, typeof(Discount), typeof(DiscountViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetGoods(UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.Goods.GetAllIncludingCategoryAndMaker();

            var baseQuery = query
                .OrderByDescending(g => g.Id)
                .DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)Mapper.Map(e, typeof(Good), typeof(GoodViewModel))).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetPosScreenTemplateViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.PosScreenTemplates.GetAllIncludingPointsOfSaleOrderedByName();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery.Select(e => (BaseViewModel)new PosScreenTemplateViewModel
            {
                Name = e.Name,
                AppliedTo = e.PointsOfSale.Any()
                    ? string.Join(", ", e.PointsOfSale.Select(p => p.AbbreviatedName.Trim()).ToList())
                    : "",
                Id = e.Id
            }).ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetLabeledGoodsGroupByPosViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            return new LabeledGoodsGroupByGoodViewModelManager(_unitOfWork).GetLabeledGoodsGroupByGoodViewModel(parameters);
        }

        private ReferenceDataWithPagination GetRolesViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.Roles.GetAll();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery
                .Select(e => (BaseViewModel)Mapper
                    .Map(e, typeof(Role), typeof(RoleViewModel)))
                .ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }

        private ReferenceDataWithPagination GetAppFeaturesViewModel(
            UniReferenceDataProviderGetParameters parameters)
        {
            var query = _unitOfWork.AppFeatureItems.GetAll();

            var baseQuery = query.DynamicFilter(parameters.Filter, parameters.Pagination);

            var baseList = baseQuery
                .OrderBy(x => x.PermissionCategory.GetDescription())
                .Select(e => (BaseViewModel)Mapper
                    .Map(e, typeof(AppFeatureItem), typeof(AppFeaturesViewModel)))
                .ToList();

            var queryAsPaginationInfo = baseQuery as IPaginationInfo;
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination { Data = baseList, PaginationInfo = paginationInfo };
        }
    }
}
