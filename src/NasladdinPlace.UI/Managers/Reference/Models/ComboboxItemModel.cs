using System;
using NasladdinPlace.UI.Managers.Reference.Interfaces;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class ComboboxItemModel : BaseViewModel, IComboboxViewModel
    {
        public object Value { get; set; }
        public string Text { get; set; }

        public ComboboxItemModel()
        {
        }

        public ComboboxItemModel(object value, string text)
        {
            Text = text;
            Value = value;
        }

        public string GetText()
        {
            return Text;
        }

        public object GetValue()
        {
            return Value;
        }

        public override Type EntityType()
        {
            throw new NotSupportedException();
        }
    }    
}
