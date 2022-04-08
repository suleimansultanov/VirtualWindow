using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.PosOperationTransaction
{
    public class PosOperationTransactionViewModel : BaseViewModel
    {
        [Display(Name = "Дата транзакции")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime CreatedDate { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int PosOperationTransactionId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int PosOperationId { get; set; }

        [Display(Name = "Тип транзакции")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosOperationTransactionType), ReadOnly = true)]
        public int Type { get; set; }

        [Display(Name = "Статус транзакции")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosOperationTransactionStatus), ReadOnly = true)]
        public int Status { get; set; }

        [Display(Name = "Оплата бонусами")]
        [Render(Control = RenderControl.Decimal)]
        public decimal BonusAmount { get; set; }

        [Display(Name = "Оплата деньгами")]
        [Render(Control = RenderControl.Decimal)]
        public decimal MoneyAmount { get; private set; }

        [Display(Name = "Сумма на фискализацию")]
        [Render(Control = RenderControl.Decimal)]
        public decimal FiscalizationAmount { get; private set; }

        [Display(Name = "Дата оплаты эквайринг")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? BankTransactionPaidDate { get; set; }

        [Display(Name = "Дата фискализации")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? FiscalizationPaidDate { get; set; }

        [Display(Name = "Общая стоимость без учета скидок и бонусов")]
        [Render(Control = RenderControl.Decimal)]
        public decimal TotalCost { get; private set; }

        [Display(Name = "Общая сумма скидки")]
        [Render(Control = RenderControl.Decimal)]
        public decimal TotalDiscountAmount { get; private set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Core.Models.PosOperationTransaction);
        
    }
}
