using System;
using System.Text;
using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.Managers.Reference.Attributes
{
    /// <summary>
    /// Аттрибут для настройки рендеринга 
    /// </summary>
    public class RenderAttribute : Attribute
    {
        public const string AdditionalValuesKey = "RenderAttribute";

        /// <summary>
        /// Название свойства. Заполняется автоматически
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Отображение поля (по умолчанию в гриде)
        /// </summary>
        public DisplayType DisplayType { get; set; } = DisplayType.InGrid;

        /// <summary>
        /// Тип контрола
        /// </summary>
        public RenderControl Control { get; set; } = RenderControl.Input;


        private Type _comboSource;
        private string _comboSourceFullName;

        /// <summary>
        /// Заполняется только для <see cref="RenderControl.Combo"/> и <see cref="RenderControl.ComboEmpty"/>
        /// </summary>
        public Type ComboSource
        {
            get { return _comboSource; }
            set
            {
                if (value == null)
                    return;
                _comboSource = value;
                _comboSourceFullName = value.FullName;
            }
        }

        /// <summary>
        /// Копия 
        /// </summary>
        public string ComboSourceFullName
        {
            get
            {
                return _comboSource != null
                    ? _comboSource.FullName
                    : _comboSourceFullName;
            }
            set { _comboSourceFullName = value; }
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Поле должно быть не пустым
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Поле не используется
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Параметр сортировки для выпадающих списков
        /// </summary>
        public string SortMap { get; set; }

        /// <summary>
        /// Зависимое поле
        /// </summary>
        public string DependencyProperty { get; set; }

        /// <summary>
        /// Разрешение редактирования в зависимости от условия
        /// </summary>
        public string EnableCondition { get; set; }

        /// <summary>
        /// Название фильтра. Если отсутствует, то используется <see cref="PropertyName"/>
        /// </summary>
        public string FilterName { get; set; }

        /// <summary>
        /// Состояние фильтра
        /// </summary>
        public FilterState FilterState { get; set; } = FilterState.NotSet;

        /// <summary>
        /// Параметры сортировки
        /// </summary>
        public SortState SortState { get; set; } = SortState.NotSet;

        /// <summary>
        /// Текстовое описание
        /// </summary>
        public string TextReference { get; set; }

        /// <summary>
        /// Заполняется только для <see cref="RenderControl.TextReference"/>
        /// </summary>
        public TextReferenceSources TextReferenceSource { get; set; }

        /// <summary>
        /// Признак Reaonly
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Активность
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Добавить Id в атрибуты
        /// </summary>
        public bool IsAddIdName { get; set; } = false;

        /// <summary>
        /// Максимальная длина
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// мин значение
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// макс значение
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Имя паршл вью
        /// </summary>
        public string PartialName { get; set; }

        /// <summary>
        /// Запрет на очистку значения фильтра
        /// </summary>
        public bool IsNotCleanValue { get; set; } = false;

        /// <summary>
        /// Порядок в котором будет отображаться фильтр
        /// </summary>
        public int FilterOrder { get; set; }

        /// <summary>
        /// Получить источник данных комбо контролла
        /// </summary>        
        /// <returns></returns>
        public string GetComboDataSource(bool forFilter = false)
        {
            if (string.IsNullOrEmpty(DependencyProperty))
            {
                return $"$root.References['{ComboSourceFullName}'].Data";
            }

            return forFilter
                ? $"{{root : $root, referenceName:'{ComboSourceFullName}', dependency: $data.{DependencyProperty} ? $data.{DependencyProperty}.Value : null }}"
                : $"{{root : $root, referenceName:'{ComboSourceFullName}', dependency: $data.{DependencyProperty} ? $data.{DependencyProperty} : null }}";
        }

        /// <summary>
        /// Получить биндинг на источник данных комбо контролла
        /// </summary>        
        /// <returns></returns>
        public string GetComboDataSourceBinding()
        {
            return string.IsNullOrEmpty(DependencyProperty) ? "options" : "dependencyOptions";
        }

        /// <summary>
        /// Получить выражение Enable
        /// </summary>        
        public string GetEnableCondition(string binding = "enable", string defCondition = "", bool isFirstBinding = false)
        {
            var condition = (isFirstBinding ? " " : ", ") + binding + ": ";
            if (Disabled) return condition + "false";
            if (string.IsNullOrEmpty(EnableCondition)) return string.IsNullOrEmpty(defCondition) ? "" : condition + defCondition;
            return condition + EnableCondition;
        }

        /// <summary>
        /// Получить текстовое описание
        /// </summary>
        public string GetTextReference()
        {
            return string.IsNullOrEmpty(SortMap)
                ? TextReference
                : SortMap;
        }

        /// <summary>
        /// Получить параметры сортировки
        /// </summary>        
        public string GetSortParameter()
        {
            return string.IsNullOrEmpty(SortMap) ? PropertyName : SortMap;
        }

        /// <summary>
        /// Получить кастомные html аттрибуты
        /// </summary>        
        public string GetHtmlAttributes()
        {
            var htmlAttr = new StringBuilder();
            htmlAttr.Append($" name=\"{PropertyName}\"");
            if (IsAddIdName) htmlAttr.Append($" id=\"{PropertyName}\"");
            if (ReadOnly) htmlAttr.Append(" readonly=\"readonly\"");
            if (Disabled) htmlAttr.Append(" disabled=\"disabled\"");
            if (Required) htmlAttr.Append(" required=\"true\"");
            if (MaxLength > 0) htmlAttr.AppendFormat(" maxlength = \"{0}\"", MaxLength);
            if (!string.IsNullOrEmpty(Min)) htmlAttr.Append($" min= \"{Min}\"");
            if (!string.IsNullOrEmpty(Max)) htmlAttr.Append($" max= \"{Max}\"");
            return htmlAttr.ToString();
        }

        public bool NeedLoadReference => ComboSource != null;
    }
}
