using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Media
{
    public class MediaContentToPosPlatformViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int MediaContentToPosPlatformId { get; set; }

        [Display(Name = "Дата добавления")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateTimeCreated { get; set; }

        [Render(Control = RenderControl.TextReferences, TextReference = "FileName", FilterState = FilterState.Disable)]
        [Display(Name = "Имя файла / Ссылка")]
        public int MediaContentId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string FileName { get; set; }

        [Display(Name = "Тип медиа контента витрины")]
        [Render(Control = RenderControl.ComboEmpty, ComboSource = typeof(PosScreenType))]
        public int PosScreenType { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editRemoveActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(MediaContentToPosPlatform);
    }
}
