using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Dtos.Check;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Checks;
using NasladdinPlace.UI.ViewModels.PosOperationTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.UI.ViewModels.Documents;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    public class GoodsMovingController : ReferenceBaseController
    {
        private readonly IDetailedCheckMaker _detailedCheckMaker;

        public GoodsMovingController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _detailedCheckMaker = serviceProvider.GetRequiredService<IDetailedCheckMaker>();
        }

        [HttpGet]
        [Route("/Checks/{operationId:int}")]
        [Permission(nameof(DocumentGoodsMovingPermission))]
        public async Task<IActionResult> Details(int operationId)
        {
            var posOperation = await UnitOfWork.PosOperations.GetIncludingCheckItemsAsync(operationId);

            if (posOperation == null)
                return this.RedirectToHome();

            var viewModel = Mapper.Map<DetailedCheckViewModel>(posOperation);

            var detailedCheck = _detailedCheckMaker.MakeCheck(posOperation);

            viewModel.Check = Mapper.Map<DetailedCheckDto>(detailedCheck);

            return View(viewModel);
        }

        [HttpGet]
        [Route("/[controller]/[action]/{operationTransactionId:int}")]
        [Permission(nameof(DocumentGoodsMovingPermission))]
        public async Task<IActionResult> TransactionDetails(int operationTransactionId)
        {
            var posOperationTransaction =
                await UnitOfWork.PosOperationTransactions.GetByIdIncludingBankAndFiscalisationInfosAsync(operationTransactionId);

            if (posOperationTransaction == null)
                return this.RedirectToHome();

            var viewModel = Mapper.Map<PosOperationTransactionDetailsViewModel>(posOperationTransaction);

            return View(viewModel);
        }

        [HttpGet]
        [Route("/[controller]/[action]/{posOperationId:int}")]
        [Permission(nameof(DocumentGoodsMovingPermission))]
        public async Task<IActionResult> DocumentGoodsMoving(int posOperationId)
        {
            var documentGoodsMoving =
                await UnitOfWork.DocumentsGoodsMoving.GetByPosOperationIdIncludingTablePartPosOperationPosUserAndGoodAsync(posOperationId);

            if (documentGoodsMoving == null)
                return this.RedirectToHome();

            var viewModel = Mapper.Map<DocumentGoodsMovingViewModel>(documentGoodsMoving);

            return View(viewModel);
        }

        [HttpGet]
        [Permission(nameof(DocumentGoodsMovingPermission))]
        public async Task<IActionResult> Add(int operationId, int posId)
        {
            var permittedLabeledGoodsForAddition =
                await UnitOfWork.LabeledGoods.GetEnabledExceptUsedInPosOperationCheckItems(operationId) ?? new List<LabeledGood>();

            var selectListPermittedLabeledGoods = new SelectList(permittedLabeledGoodsForAddition
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Good.Id} - {p.Good.Name}, {p.Label}, {p.Price} {p.Currency.Name}"
                    }), "Value", "Text");

            var checkItemAdditionFormViewModel = new CheckItemAdditionFormViewModel
            {
                LabeledGoodItems = selectListPermittedLabeledGoods
            };

            return PartialView("AddItemForm", checkItemAdditionFormViewModel);
        }

        [HttpGet]
        [Route("/[controller]")]
        [Permission(nameof(ReadOnlyAccess))]
        public IActionResult GetChecks(PosOperationFiltersContext filtersContext)
        {
            var context = GetFilterContext(filtersContext);

            var operationDateFrom = filtersContext.OperationDateFrom?.ToDynamicFilterDateFormat();
            var operationDateUntil = filtersContext.OperationDateUntil?.ToDynamicFilterDateFormat();
            var auditRequestDateFrom = filtersContext.AuditRequestDateTimeFrom?.ToDynamicFilterDateFormat();
            var auditRequestDateUntil = filtersContext.AuditRequestDateTimeUntil?.ToDynamicFilterDateFormat();

            return Reference<PosOperationViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Движение ТМЦ",
                    BeforeGridPartialName = "~/Views/GoodsMoving/_beforeGridPosOperations.cshtml",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilterFromToDateTime(filter, topFilter,
                            nameof(PosOperationViewModel.OperationDateTime), operationDateFrom, operationDateUntil,
                            "Дата покупки от", "Дата покупки до");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(PosOperationViewModel.AuditRequestDateTime), auditRequestDateFrom, auditRequestDateUntil, "Дата запроса аудита c",
                            "Дата запроса аудита по");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(PosOperationViewModel.AuditCompletionDateTime), null, null, "Дата завершения аудита c",
                            "Дата завершения аудита по");
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        if (filtersContext.OperationMode != null)
                            filter.SetValueFilter(nameof(PosOperationViewModel.PosOperationMode), (int)filtersContext.OperationMode);
                    }
                });
        }

        [HttpGet]
        [Permission(nameof(DocumentGoodsMovingPermission))]
        public IActionResult CheckEditingConfirmationDialog(CheckSummaryViewModel checkSummaryViewModel)
        {
            return PartialView("CheckEditingConfirmationForm", checkSummaryViewModel);
        }

        private List<FilterItemModel> GetFilterContext(PosOperationFiltersContext filterContext)
        {
            var context = new List<FilterItemModel>();

            if (filterContext.OperationStatus.HasValue && filterContext.OperationStatusFilterType.HasValue)
            {
                context.Add(
                    new FilterItemModel
                    {
                        Value = ((int)filterContext.OperationStatus.Value).ToString(),
                        FilterType = filterContext.OperationStatusFilterType.Value,
                        ForceCastType = CastTypes.Int32,
                        PropertyName = nameof(PosOperationViewModel.FilterContextStatus),
                        FilterName = nameof(PosOperationViewModel.FilterContextStatus)
                    });
            }

            if (filterContext.TotalPrice.HasValue && filterContext.TotalPriceFilterType.HasValue)
            {
                context.Add(
                    new FilterItemModel
                    {
                        Value = filterContext.TotalPrice.Value.ToString(),
                        FilterType = filterContext.TotalPriceFilterType.Value,
                        PropertyName = nameof(PosOperationViewModel.FilterContextTotalPrice),
                        FilterName = nameof(PosOperationViewModel.FilterContextTotalPrice)
                    });
            }

            if (filterContext.HasUnverifiedCheckItems.HasValue)
            {
                context.Add(
                    new FilterItemModel
                    {
                        Value = filterContext.HasUnverifiedCheckItems.Value.ToString(),
                        FilterType = FilterTypes.Equals,
                        PropertyName = nameof(PosOperationViewModel.HasUnverifiedCheckItems),
                        FilterName = nameof(PosOperationViewModel.HasUnverifiedCheckItems)
                    }
                );
            }

            if (filterContext.HasFiscalizationInfoErrors.HasValue)
            {
                context.Add(
                    new FilterItemModel
                    {
                        Value = filterContext.HasFiscalizationInfoErrors.Value.ToString(),
                        FilterType = FilterTypes.Equals,
                        PropertyName = nameof(PosOperationViewModel.HasFiscalizationInfoErrors),
                        FilterName = nameof(PosOperationViewModel.HasFiscalizationInfoErrors)
                    }
                );
            }

            return context;
        }
    }
}