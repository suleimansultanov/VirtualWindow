using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Reports
{
    public class ReportUploadingInfoViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Адрес таблицы")]
        public string Url { get; set; }

        [Display(Name = "Название листа")]
        public string Sheet { get; set; }

        [Display(Name = "Тип задачи")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(ReportType))]
        public int Type { get; set; }

        [Render(FilterState = FilterState.Disable, SortState = SortState.Disable, Control = RenderControl.TextArea)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Render(FilterState = FilterState.Disable, SortState = SortState.Disable, Control = RenderControl.TextArea)]
        [Display(Name = "Количество строк в запросе")]
        public int BatchSize { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/ReportsUploadingInfos/_reportsActionsControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(ReportUploadingInfo);
    }
}
