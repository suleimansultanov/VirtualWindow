using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Makers
{
    public class MakerViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editRemoveActionControl.cshtml")]
        public string Action { get; set; }

        public override Type EntityType() => typeof(Maker);
    }
}
