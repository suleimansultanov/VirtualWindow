using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Dtos.Currency;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.LabeledGoods;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class LabeledGoodsController : ReferenceBaseController
    {
        public LabeledGoodsController(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        [Route("PointsOfSale/{posId}/LabeledGoods/Identification", Order = 1)]
        [Permission(nameof(LabeledGoodManagementPermission))]
        public async Task<IActionResult> Identify(int posId = 0)
        {
            if (posId == 0)
            {
                if (!HttpContext.Session.TryGetCurrentPosId(out posId))
                {
                    posId = 1;
                }

                return RedirectToAction("Identify", new { posId });
            }

            var labeledGoodsUntiedFromGood = await UnitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(posId);

            var pointsOfSale = await UnitOfWork.PointsOfSale.GetAvailableForIdentificationAsync();

            if (!pointsOfSale.Any())
                return this.RedirectToHome();

            var goodsResult = await UnitOfWork.Goods.GetAllAsync();

            var selectedPos = pointsOfSale.SingleOrDefault(pos => pos.Id == posId);

            if (selectedPos == null)
                return RedirectToAction("Identify", new { posId = pointsOfSale.First().Id });

            var viewModel = new LabeledGoodsToGoodFormViewModel
            {
                PosSelectList = pointsOfSale.ToSelectList(),
                PosId = posId,
                Labels = labeledGoodsUntiedFromGood.Select(lg => lg.Label),
                GoodSelectList = goodsResult.Select(g => new GoodDto
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToSelectList()
            };

            return View("LabelsToGoodAssignmentForm", viewModel);
        }

        [Permission(nameof(LabeledGoodManagementPermission))]
        public async Task<IActionResult> EditLabeledGoodAsync(int id)
        {
            var labeledGood = await UnitOfWork.LabeledGoods.GetByIdAsync(id);
            if (labeledGood == null)
                return this.RedirectToHome();

            var viewModel = Mapper.Map<LabeledGoodFormViewModel>(labeledGood);
            ViewBag.ReturnUrl = Request.Headers["Referer"].ToString();

            return await AddGoodsAndCurrenciesSelectListAndReturnLabeledGoodFormAsync(viewModel);
        }

        [HttpGet]
        [Permission(nameof(LabeledGoodManagementPermission))]
        public IActionResult TrackingHistory(TrackingHistoryLabeledGoodsFiltersContext filtersContext)
        {
            var context = new List<FilterItemModel>();

            var fromDateTime = filtersContext.TrackingHistoryFrom?.ToDynamicFilterDateFormat();
            var toDateTime = filtersContext.TrackingHistoryUntil?.ToDynamicFilterDateFormat();

            if (filtersContext.Id.HasValue)
                context.Add(new FilterItemModel
                {
                    Value = filtersContext.Id.Value.ToString(),
                    FilterType = FilterTypes.Equals,
                    ForceCastType = CastTypes.Int32,
                    PropertyName = nameof(LabeledGoodTrackingRecordViewModel.LabeledGoodId),
                    FilterName = nameof(LabeledGoodTrackingRecordViewModel.LabeledGoodId)
                });

            return Reference<LabeledGoodTrackingRecordViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Потерянные и найденные метки",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilterFromToDateTime(filter, topFilter,
                            nameof(LabeledGoodTrackingRecordViewModel.Timestamp), fromDateTime, toDateTime,
                            "Дата появления от", "Дата появления до");
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(LabeledGoodTrackingRecordViewModel.Timestamp), SortTypes.Desc);
                    }
                });
        }

        [HttpGet]
        public IActionResult GetLabeledGoodsGrouppedByGood(OverdueGoodsFilterContext filtersContext)
        {
            return Reference<LabeledGoodsGroupByGoodViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Экземпляры",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-bolt'></em> Запросить",
                            Binding = "click: $root.sendPosContentRequest"
                        }
                    },
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilter(topFilter, filter,
                            new RenderAttribute
                            {
                                Control = RenderControl.Combo,
                                ComboSource = typeof(PosBasicInfoViewModel),
                                DisplayName = "Витрины",
                                PropertyName = nameof(OverdueGoodsFilterContext.PosId),
                            }, filtersContext.PosId.HasValue ? filtersContext.PosId.Value.ToString() : string.Empty);
                        ConfigReference.CreateFilter(topFilter, filter,
                            new RenderAttribute
                            {
                                Control = RenderControl.Combo,
                                ComboSource = typeof(OverdueType),
                                DisplayName = "Срок годности",
                                PropertyName = nameof(OverdueGoodsFilterContext.Type),
                            }, filtersContext.Type.HasValue ? ((int)filtersContext.Type.Value).ToString() : string.Empty);
                    },
                    AdditionReferenceSource = new List<Type> { typeof(PosBasicInfoViewModel), typeof(OverdueType) },
                    BeforeGridPartialName = "~/Views/LabeledGoods/_beforeGridLabeledGoodsGroupByGoodPartial.cshtml",
                    AfterGridPartialName = "~/Views/LabeledGoods/_afterGridLabeledGoodsGroupByGoodPartial.cshtml"
                });
        }

        public IActionResult Disabled()
        {
            return View();
        }

        private async Task<IActionResult> AddGoodsAndCurrenciesSelectListAndReturnLabeledGoodFormAsync(
            LabeledGoodFormViewModel viewModel)
        {
            var goods = (await UnitOfWork.Goods.GetAllAsync()).Select(Mapper.Map<GoodDto>);
            var currencies = (await UnitOfWork.Currencies.GetAllAsync()).Select(Mapper.Map<CurrencyDto>);

            viewModel.GoodSelectList = goods.ToSelectList();
            viewModel.CurrencySelectList = currencies.ToSelectList();

            return View("LabeledGoodForm", viewModel);
        }
    }
}