using System;
using NasladdinPlace.UI.Managers.Reference.Interfaces;
using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    /// <summary>
    /// Модель для хранения одного элемента справочника
    /// </summary>
    public class ReferenceItemModel
    {

        /// <summary>
        /// Зависимое значение
        /// </summary>
        public object Dependency { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        public string Text { get; set; }

        public ReferenceItemModel()
        {
        }

        public ReferenceItemModel(object value, string text, object dependency = null)
        {
            Dependency = dependency;
            Value = value;
            Text = text;
        }

        public ReferenceItemModel(IComboboxViewModel viewModel)
        {
            Value = viewModel.GetValue();
            Text = viewModel.GetText();
        }

        public ReferenceItemModel(IDependencyComboboxViewModel viewModel) : this(viewModel as IComboboxViewModel)
        {
            Dependency = viewModel.GetDependency();
        }

        /// <summary>
        /// Создать ReferenceItemModel по значениею енума 
        /// </summary>
        public static ReferenceItemModel Create(Enum @enum)
        {
            return new ReferenceItemModel
            {
                Text = @enum.GetDescription(),
                Value = @enum
            };
        }

    }
}
