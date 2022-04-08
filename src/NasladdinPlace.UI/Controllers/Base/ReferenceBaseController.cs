using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Base;
using ReferenceManager = NasladdinPlace.UI.Managers.Reference.ReferenceManager;

namespace NasladdinPlace.UI.Controllers.Base
{
    public class ReferenceBaseController : BaseController
    {
        private readonly ReferenceManager _referenceManager;

        public ReferenceBaseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
            _referenceManager = new ReferenceManager(UnitOfWork, nasladdinApiClient);
        }

        /// <summary>
        /// Получение данных справочника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Load([FromBody] ReferenceLoadModal model)
        {          
            var type = Type.GetType(model.ReferenceType);
            if (type == null)
                return BadRequest("Не верный параметр referenceType");

            return JsonContent(new
            {
                status = "ok",
                message = "Данные успешно загружены!",
                data = GetDataViewModel(type, context: model.Context, filters: model.Filter, pagination: model.Pagination)
            });            
        }

        protected IActionResult Reference<T>(ICollection<FilterItemModel> context = null, 
                                             string viewName = "~/Views/Reference/Reference.cshtml", 
                                             ConfigReference configuration = null) where T : BaseViewModel
        {
            ViewBag.TextReference = true;
            var viewModel = UniReferenceViewModel<T>(context, configuration);
            return View(viewName, viewModel);            
        }

        protected UniReferenceViewModel<BaseViewModel> UniReferenceViewModel<T>(ICollection<FilterItemModel> context = null, ConfigReference configuration = null) where T : BaseViewModel
        {
            configuration = configuration ?? new ConfigReference();
            var entityType = typeof(T);

            List<FilterItemModel> filterList = null;

            if (configuration.SetDefaultValueFilter != null || configuration.CreateAdditionFilter != null)
            {
                var infos = new FormRendererHelper().GetFields(entityType).ToList();
                var filterDictionary = infos.GetFilterItems();

                configuration.CreateAdditionFilter?.Invoke(filterDictionary, configuration.Filters);

                configuration.SetDefaultValueFilter?.Invoke(filterDictionary, infos);

                filterList = filterDictionary.Select(x => x.Value).ToList();
            }

            var viewModel = GetDataViewModel(entityType, 
                                             context, 
                                             filterList, 
                                             new PaginationInfo(1, 20), 
                                             configuration.AdditionReferenceSource, 
                                             configuration.IsAutoLoadData);

            configuration.CreateAdditionFilter?.Invoke(viewModel.Filter, new List<RenderAttribute>());

            viewModel.Configuration = configuration;
            viewModel.ContextFields = context?.Select(x => x.PropertyName).ToList();

            if (configuration.BreadCrumbs == null)
                configuration.BreadCrumbs = new List<string> { GetBreadCrumb(Url.Action("All", "PointsOfSale"), "Домой") };
            
            return viewModel;
        }

        /// <summary>
        /// Возвращает viewmodel справочника
        /// </summary>
        /// <returns></returns>
        protected UniReferenceViewModel<BaseViewModel> GetDataViewModel(Type entityType, 
                                                                        ICollection<FilterItemModel> context = null, 
                                                                        List<FilterItemModel> filters = null,
                                                                        PaginationInfo pagination = null,
                                                                        ICollection<Type> additionReferenceSource = null,
                                                                        bool isAutoLoadData = true)
        {
            if (context != null)
                filters?.InsertRange(0, context);

            var dataParameters = new ReferenceGetDataParametersModel(entityType, filters ?? context, pagination, additionReferenceSource, isAutoLoadData);

            var referenceData = _referenceManager.GetData(dataParameters);
            var references = _referenceManager.GetReferences(dataParameters);

            var viewModel = new UniReferenceViewModel<BaseViewModel>(dataParameters.ViewModel, referenceData.Data, references)
            {
                Url = Url.Action("", ControllerName),
                LoadUrl = GetLoadUrl(),
                Context = context,
                Pagination = referenceData.PaginationInfo,
                CustomData = referenceData.CustomData
            };

            filters?.ForEach(f =>
            {
                if (!viewModel.Filter.TryGetValue(f.FilterName ?? f.PropertyName, out var filter)) return;
                if (f.Value != null) filter.Value = f.Value;
                if (f.Sort != null)
                {
                    filter.Sort = f.Sort;
                    filter.SortType = f.SortType;
                    filter.SortOrder = f.SortOrder;
                }
            });

            return viewModel;
        }

        /// <summary>
        /// Ссылка на метод Load в текущем контролере
        /// </summary>
        /// <returns></returns>
        private string GetLoadUrl()
        {
            return Url.Action("Load", ControllerName);
        }

        public string GetBreadCrumb(string url, string name)
        {
            return $"<a href='{url}'>{name}</a>";
        }
    }
}