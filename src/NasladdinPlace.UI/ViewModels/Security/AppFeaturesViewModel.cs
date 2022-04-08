using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;

namespace NasladdinPlace.UI.ViewModels.Security
{
    public class AppFeaturesViewModel : BaseViewModel
    {
        [LocalizedRequired]
        [Render(Control = RenderControl.TextArea, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        [Display(Name = "Категория")]
        public string Category { get; set; }

        [LocalizedRequired]
        [Display(Name = "Разрешение")]
        public string Description { get; set; }

        public override Type EntityType() => typeof(AppFeatureItem);
    }
}
