using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.PosScreenTemplates
{
    public class PosScreenTemplateViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Применяется к витринам")]
        [Render(Control = RenderControl.Input, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public string AppliedTo { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, SortState = SortState.Disable, PartialName = "~/Views/Shared/Actions/_editRemoveActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Core.Models.PosScreenTemplate);
    }
}