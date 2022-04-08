using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.PosSensor;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class PosSensorsController : ReferenceBaseController
    {
        public PosSensorsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public IActionResult GetPosAbnormalSensorMeasurement(PosAbnormalSensorMeasurementFiltersContext filtersContext)
        {
            var abnormalSensorFilterDateFrom = filtersContext.PosAbnormalSensorMeasurementDateFrom?.ToDynamicFilterDateFormat();
            var abnormalSensorFilterDateUntil = filtersContext.PosAbnormalSensorMeasurementDateUntil?.ToDynamicFilterDateFormat();

            var breadCrumbs = new List<string>
            {
                GetBreadCrumb(Url.Action("All", "PointsOfSale"), "Домой")
            };

            var context = new List<FilterItemModel>();

            if (filtersContext.PosId.HasValue)
            {
                breadCrumbs.Add(GetBreadCrumb(Url.Action("EditPos", "PointsOfSale", new { id = filtersContext.PosId }),
                    "Карточка витрины"));

                context.Add(new FilterItemModel
                {
                    Value = filtersContext.PosId.Value.ToString(),
                    FilterType = FilterTypes.Equals,
                    ForceCastType = CastTypes.Int32,
                    PropertyName = nameof(PosAbnormalSensorMeasurementViewModel.PosId),
                    FilterName = nameof(PosAbnormalSensorMeasurementViewModel.PosId)
                });
            }

            return Reference<PosAbnormalSensorMeasurementViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Зафиксированые сбои датчиков на витринах",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilterFromToDateTime(filter, topFilter,
                            nameof(PosAbnormalSensorMeasurementViewModel.DateMeasured), abnormalSensorFilterDateFrom, abnormalSensorFilterDateUntil,
                            "Дата фиксации от", "Дата фиксации до");
                    },
                    BreadCrumbs = breadCrumbs,
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(PosAbnormalSensorMeasurementViewModel.DateMeasured), SortTypes.Desc);
                    }
                });
        }
    }
}