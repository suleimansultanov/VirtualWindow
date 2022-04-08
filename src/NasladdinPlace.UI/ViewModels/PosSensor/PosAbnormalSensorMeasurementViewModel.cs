using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.PosSensor
{
    public class PosAbnormalSensorMeasurementViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Витрина")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "PosName", TextReferenceSource = TextReferenceSources.PointOfSales)]
        public int? PosId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string PosName { get; set; }

        [Display(Name = "Дата фиксации")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateMeasured { get; set; }

        [Display(Name = "Тип датчика")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PosSensorType))]
        public int Type { get; set; }

        [Display(Name = "Значение")]
        [Render(Control = RenderControl.TextArea, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public string Value { get; set; }

        [Display(Name = "Расположение")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(SensorPosition))]
        public int SensorPosition { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Entity);

    }
}