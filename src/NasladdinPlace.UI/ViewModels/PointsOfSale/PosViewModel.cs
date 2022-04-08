using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Название витрины")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_linkNameActionControl.cshtml")]
        public string Name { get; set; }

        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "City", TextReferenceSource = TextReferenceSources.Cities)]
        [Display(Name = "Город")]
        public int CityId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string City { get; set; }

        [Display(Name = "Улица")]
        public string Street { get; set; }

        [Display(Name = "Статус витрины")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/PointsOfSale/_renderConnectionStatus.cshtml")]
        public string Status { get; set; }

        [Display(Name = "IP-адрес")]
        [Render(Control = RenderControl.TextArea, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public string IpAddress { get; set; }
        
        [Display(Name = "Версия")]
        [Render(Control = RenderControl.TextArea, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public string Version { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public List<int> AssignedRoles { get; set; }

        [Display(Name = "Режим эксплуатации")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/PointsOfSale/_renderPosActivityStatus.cshtml")]
        public PosActivityStatus? PosActivityStatus { get; set; }

        [Display(Name = "Отобразить неактивные витрины?")]
        [Render(DisplayType = DisplayType.Hide, Control = RenderControl.YesNo,  Required = true)]
        public bool IsNotDeactivated { get; set; }

        public override Type EntityType() => typeof(Entity);
        
    }
}
