using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Media
{
    public class MediaContentViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int MediaContentId { get; set; }

        [Display(Name = "Дата добавления")]
        [Render(Control = RenderControl.DateTime, FilterState =  FilterState.Disable)]
        public DateTime UploadDateTime { get; set; }

        [Display(Name = "Имя файла / Ссылка")]
        public string FileName { get; set; }

        [Display(Name = "Тип файла")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(MediaContentType))]
        public int ContentType { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public byte[] FileContent { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_removeActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(MediaContent);
    }
}
