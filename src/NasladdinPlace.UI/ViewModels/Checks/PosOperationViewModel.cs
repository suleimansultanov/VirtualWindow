using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class PosOperationViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int PosOperationId { get; set; }

        [Display(Name = "Режим")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosMode))]
        public int PosOperationMode { get; set; }

        [Display(Name = "Дата покупки")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public DateTime OperationDateTime { get; set; }

        [Display(Name = "Имя пользователя")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "UserName", TextReferenceSource = TextReferenceSources.Users, FilterName = nameof(UserId))]
        public int? UserId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string UserName { get; set; }

        [Display(Name = "Витрина")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "PosName", TextReferenceSource = TextReferenceSources.PointOfSales, FilterName = nameof(PosId))]
        public int? PosId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string PosName { get; set; }

        [Display(Name = "Сумма к оплате")]
        [Render(Control = RenderControl.Decimal, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public decimal TotalPriceWithoutDiscountAndBonus { get; set; }

        [Display(Name = "Статус операции")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosOperationStatus), FilterName = nameof(Status))]
        public int Status { get; set; }

        [Display(Name = "Способы оплаты")]
        [Render(DisplayType = DisplayType.Hide, FilterState = FilterState.Disable)]
        public IEnumerable<PaymentCardCryptogramSource> CryptogramSources { get; set; }

        [Display(Name = "Способы оплаты")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PaymentCardCryptogramSource), DisplayType = DisplayType.BelowGrid, FilterName = nameof(FilterPaymentCardCryptogramSource))]
        public int FilterPaymentCardCryptogramSource { get; set; }

        [Display(Name = "Статус фискализации")]
        [Render(DisplayType = DisplayType.Hide, Control = RenderControl.ComboEmpty, ComboSource = typeof(FiscalizationState), FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public int? FiscalizationState { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int FilterContextStatus { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int FilterContextTotalPrice { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public bool HasDocumentsGoodsMoving { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public bool IsDocumentGoodsMovingHasUntiedItems { get; set; }

        [Display(Name = "Содержит условные покупки?")]
        [Render(Control = RenderControl.YesNo, FilterName = nameof(HasUnverifiedCheckItems))]
        public bool HasUnverifiedCheckItems { get; set; }

        [Display(Name = "Содержит проблемы фискализации?")]
        [Render(Control = RenderControl.YesNo, FilterName = nameof(HasFiscalizationInfoErrors))]
        public bool HasFiscalizationInfoErrors { get; set; }

        [Display(Name = "Дата запроса аудита")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? AuditRequestDateTime { get; set; }

        [Display(Name = "Дата завершения аудита")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? AuditCompletionDateTime { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/GoodsMoving/_actionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(PosOperation);
    }
}
