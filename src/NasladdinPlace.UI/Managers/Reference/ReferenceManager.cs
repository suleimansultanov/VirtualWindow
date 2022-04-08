using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Managers.Reference.Interfaces;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Base;
using NasladdinPlace.UI.ViewModels.Checks;
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
using NasladdinPlace.UI.ViewModels.Users;
using PagedList.Interfaces;

namespace NasladdinPlace.UI.Managers.Reference
{
    public partial class ReferenceManager
    {
        /// <summary>
        /// Dictionary сущностей, для которых получение данных отличается от универсалного
        /// </summary>
        protected readonly DictionaryTypes<Func<UniReferenceDataProviderGetParameters, ReferenceDataWithPagination>> Map =
            new DictionaryTypes<Func<UniReferenceDataProviderGetParameters, ReferenceDataWithPagination>>();

        private readonly IUnitOfWork _unitOfWork;
        private readonly INasladdinApiClient _nasladdinApiClient;

        public ReferenceManager(IUnitOfWork unitOfWork,
            INasladdinApiClient nasladdinApiClient)
        {
            _unitOfWork = unitOfWork;
            _nasladdinApiClient = nasladdinApiClient;

            Map.Add<ScheduleViewModel>(GetScheduleViewModel);
            Map.Add<PosMediaContentViewModel>(GetPosMediaContentViewModel);
            Map.Add<PosOperationViewModel>(GetPosOperationViewModel);
            Map.Add<MediaContentViewModel>(GetMediaContentViewModel);
            Map.Add<UserViewModel>(GetUserViewModel);
            Map.Add<PosTemperatureDetailsViewModel>(GetPosTemperatureDetailsViewModel);
            Map.Add<PosViewModel>(GetPosViewModel);
            Map.Add<LabeledGoodTrackingRecordViewModel>(GetLabeledGoodTrackingRecordViewModel);
            Map.Add<PosAbnormalSensorMeasurementViewModel>(GetPosAbnormalSensorMeasurementsViewModel);
            Map.Add<PosLogViewModel>(GetPosLogsViewModel);
            Map.Add<DiscountViewModel>(GetDiscounts);
            Map.Add<GoodViewModel>(GetGoods);
            Map.Add<PosScreenTemplateViewModel>(GetPosScreenTemplateViewModel);
            Map.Add<LabeledGoodsGroupByGoodViewModel>(GetLabeledGoodsGroupByPosViewModel);
            Map.Add<RoleViewModel>(GetRolesViewModel);
            Map.Add<AppFeaturesViewModel>(GetAppFeaturesViewModel);
        }

        /// <summary>
        /// Формируем данные справочника
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ReferenceDataWithPagination GetData(ReferenceGetDataParametersModel parameters)
        {
            if (!parameters.LoadData)
            {
                return new ReferenceDataWithPagination
                {
                    PaginationInfo = parameters.Pagination,
                    Data = new List<BaseViewModel>()
                };
            }

            if (Map.ContainsKey(parameters.ViewModelType))
            {
                var filters = parameters.Filter?.ToList() ?? new List<BaseFilterItemModel>();
                return Map[parameters.ViewModelType](new UniReferenceDataProviderGetParameters(filters, parameters.Pagination, _unitOfWork));
            }

            var query = _unitOfWork.References.Get(parameters.EntityType, parameters.Filter?.ToList(), parameters.Pagination);
            var queryAsPaginationInfo = query as IPaginationInfo;

            var data = query.Select(e => (BaseViewModel)Mapper.Map(e, parameters.EntityType, parameters.ViewModelType)).ToList();
            var paginationInfo = (queryAsPaginationInfo) == null ? null : new PaginationInfo(queryAsPaginationInfo);

            return new ReferenceDataWithPagination
            {
                PaginationInfo = paginationInfo,
                Data = data
            };            
        }

        public List<ReferencesModel> GetReferences(ReferenceGetDataParametersModel parameters)
        {
            var type = parameters.ViewModelType;
            var formRendererHelper = new FormRendererHelper();
            var renderItems = formRendererHelper.GetFields(type).ToList();

            return GetReferenceDictionary(renderItems, parameters.AdditionReferenceSource);
        }

        private List<ReferencesModel> GetReferenceDictionary(IEnumerable<FormRendererItemInfo> formRendererItemInfos, List<string> additionReferenceSource = null)
        {
            var types = formRendererItemInfos
                .Where(x => x.RenderInfo.NeedLoadReference)
                .Select(x => x.RenderInfo.ComboSource)
                .ToList();

            if (additionReferenceSource != null)
            {
                types.AddRange(additionReferenceSource.Select(Type.GetType));
            }

            return types
                .GroupBy(x => x)
                .Select(x => CreateReferencesModel(x.Key)).ToList();
        }

        /// <summary>
        /// Получить источник данных.
        /// </summary>
        /// <param name="item">Тип источника данных.</param>
        /// <returns>Список элементов источника данных.</returns>
        private ReferencesModel CreateReferencesModel(Type item)
        {
            return item.IsEnum ? EnumReferenceHelper.GetEnumReference(item) : GetEntityReference(item);
        }

        private ReferencesModel GetEntityReference(Type item)
        {
            if (!typeof(IComboboxViewModel).IsAssignableFrom(item))
                throw new ArgumentException("Не удалось сформировать справочник по типу " + item);

            var viewModel = (BaseViewModel)Activator.CreateInstance(item);
            var entityType = viewModel.EntityType();

            return CreateEntityReferenceModel(item, entityType, _unitOfWork.References.GetAll(entityType));
        }

        private ReferencesModel CreateEntityReferenceModel(Type viewModelType, Type entityType, IEnumerable<Entity> items)
        {
            var notSort = typeof(IComboboxViewModelNotSorted).IsAssignableFrom(viewModelType);

            var viewModelItems = items.Select(e => (BaseViewModel)Mapper.Map(e, entityType, viewModelType)).ToList();

            return typeof(IDependencyComboboxViewModel).IsAssignableFrom(viewModelType)
                ? new ReferencesModel(viewModelType.FullName, viewModelItems.Cast<IDependencyComboboxViewModel>(), notSort)
                : new ReferencesModel(viewModelType.FullName, viewModelItems.Cast<IComboboxViewModel>(), notSort);
        }

    }
}
