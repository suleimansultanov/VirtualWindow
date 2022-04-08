using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.LabeledGoods
{
    public class LabeledGoodTrackingRecordViewModel : BaseViewModel
    {
        [Display(Name = "Витрина")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "PosName", TextReferenceSource = TextReferenceSources.PointOfSales)]
        public int? PosId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string PosName { get; set; }
        
        [Display(Name = "Тип")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(LabeledGoodTrackingRecordType))]
        public int Type { get; set; }

        [Display(Name = "Товар")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "GoodName", TextReferenceSource = TextReferenceSources.Goods)]
        public int? GoodId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string GoodName { get; set; }

        [Display(Name = "Метка")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "Label", TextReferenceSource = TextReferenceSources.LabeledGoods)]
        public int LabeledGoodId { get; set; }
        
        [Render(DisplayType = DisplayType.Hide)]
        public string Label { get; set; }
        
        [Display(Name = "Дата сообщения")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime Timestamp { get; set; }

        [Render(Ignore =  true)]
        public override Type EntityType()
        {
            return typeof(LabeledGoodTrackingRecord);
        }
    }
}