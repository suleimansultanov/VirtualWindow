using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Logs
{
    public class PosLogViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int PosLogId { get; set; }

        [Display(Name = "Витрина")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "PosName", TextReferenceSource = TextReferenceSources.PointOfSales)]
        public int PosId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string PosName { get; set; }

        [Display(Name = "Дата получения")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateTimeCreated { get; set; }

        [Display(Name = "Имя архива")]
        public string FileName { get; set; }

        [Display(Name = "Тип логов")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosLogType))]
        public PosLogType LogType { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Logs/_actionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(PosLog);
    }
}
