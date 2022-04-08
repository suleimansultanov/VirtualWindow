using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Fiscalization
{
    public class FiscalizationInfoViewModel : BaseViewModel
    {
        [Display(Name = "Дата запроса")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime RequestDateTime { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int FiscalizationInfoId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public int PosOperationId { get; set; }

        [Display(Name = "Идентификатор запроса")]
        public Guid RequestId { get; private set; }

        [Display(Name = "Статус фискализации")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(FiscalizationState), ReadOnly = true)]
        public int State { get; set; }

        [Display(Name = "Тип фискализации")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(FiscalizationType), ReadOnly = true)]
        public int Type { get; set; }

        [Display(Name = "Дата ответа")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? ResponseDateTime { get; set; }

        [Display(Name = "Ошибки фискализации")]
        [Render(Control = RenderControl.TextArea)]
        public string ErrorInfo { get; set; }

        [Display(Name = "Информация о фискализации")]
        public string DocumentInfo { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string QrCodeValue { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string QrCodeFormat { get; set; }

        [Display(Name = "Номер фискального документа")]
        public string FiscalizationNumber { get; set; }

        [Display(Name = "Номер фискального накопителя")]
        public string FiscalizationSerial { get; set; }

        [Display(Name = "Фискальный признак документа")]
        public string FiscalizationSign { get; set; }

        [Display(Name = "Дата и время формирования документа")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? DocumentDateTime { get; set; }

        [Display(Name = "Сумма коррекции")]
        [Render(Control = RenderControl.Decimal)]
        public decimal CorrectionAmount { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(FiscalizationInfo);
    }
}
