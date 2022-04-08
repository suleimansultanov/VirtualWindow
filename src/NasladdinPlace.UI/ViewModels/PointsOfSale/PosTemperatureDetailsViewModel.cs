using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosTemperatureDetailsViewModel : BaseViewModel
    {
        [Render(Control = RenderControl.Integer, DisplayType = DisplayType.Hide, FilterState = FilterState.Disable)]
        public int PosId { get; set; }

        [Display(Name = "Дата получения температуры")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateTimeTemperatureReceipt { get; set; }

        [Display(Name = "Температура")]
        [Render(Control = RenderControl.Decimal)]
        public decimal Temperature { get; set; }

        public override Type EntityType() => typeof(Entity);
    }
}
