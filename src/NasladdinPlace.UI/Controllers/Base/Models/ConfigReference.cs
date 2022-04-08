using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers.Base.Models
{
    public class ConfigReference
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; } = null;

        /// <summary>
        /// Загружать ли данные при открытии
        /// </summary>
        public bool IsAutoLoadData { get; set; } = true;

        /// <summary>
        /// Установить значения по умолчанию для фильтров
        /// </summary>
        public Action<Dictionary<string, FilterItemModel>, ICollection<FormRendererItemInfo>> SetDefaultValueFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Добавить дополнительные фильтры
        /// </summary>
        public Action<Dictionary<string, FilterItemModel>, ICollection<RenderAttribute>> CreateAdditionFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Определяет где рисовать фильтры
        /// </summary>
        public RenderFilter FilterType { get; set; } = RenderFilter.InGridHeader;

        /// <summary>
        /// Дополнительные фильтры
        /// </summary>
        public ICollection<RenderAttribute> Filters { get; set; } = new List<RenderAttribute>();

        /// <summary>
        /// Отрисовать фильтр и сортировку в заголовке таблицы
        /// </summary>
        public bool IsRenderFilter { get; set; } = true;

        /// <summary>
        /// Отрисовать дополнительно фильты в модальном окне 
        /// </summary>
        public bool IsRenderModalFilter { get; set; } = true;

        /// <summary>
        /// Имя Partial view которое рисуется над таблицей
        /// </summary>
        public string BeforeGridPartialName { get; set; }

        /// <summary>
        /// Имя Partial view которое рисуется под таблицей
        /// </summary>
        public string AfterGridPartialName { get; set; }

        /// <summary>
        /// Дополнительные источники данных для выпадающего списка
        /// </summary>
        public ICollection<Type> AdditionReferenceSource { get; set; }

        public static void CreateFilter(ICollection<RenderAttribute> filterInfo,
            Dictionary<string, FilterItemModel> filter, RenderAttribute renderAttribute, string value,
            FilterTypes filterType = FilterTypes.Equals)
        {
            filterInfo.Add(renderAttribute);
            var filterName = string.IsNullOrWhiteSpace(renderAttribute.FilterName)
                ? renderAttribute.PropertyName
                : renderAttribute.FilterName;

            if (!filter.ContainsKey(filterName))
            {
                filter.CreateAdditionFilter(CastTypes.None, filterName, renderAttribute.PropertyName, filterType, value);
            }
            else if (value != null)
            {
                filter[filterName].Value = value;
            }
        }

        /// <summary>
        /// Создание фильтра с по для даты
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="topFilter"></param>
        /// <param name="propName"></param>
        /// <param name="valFrom"></param>
        /// <param name="valTo"></param>
        /// <param name="titleFrom"></param>
        /// <param name="titleTo"></param>
        public static void CreateFilterFromTo(Dictionary<string, FilterItemModel> filter, ICollection<RenderAttribute> topFilter, string propName, string valFrom = null, string valTo = null, string titleFrom = "C", string titleTo = "По")
        {
            var dateFrom = new RenderAttribute
            {
                Control = RenderControl.Date,
                DisplayName = titleFrom,
                PropertyName = propName,
                FilterName = "DateFrom"
            };

            var dateTo = new RenderAttribute
            {
                Control = RenderControl.Date,
                DisplayName = titleTo,
                PropertyName = propName,
                FilterName = "DateTo"
            };

            CreateFilter(topFilter, filter, dateFrom, valFrom, FilterTypes.GreaterOrEquals);
            CreateFilter(topFilter, filter, dateTo, valTo, FilterTypes.LessOrEquals);
        }

        /// <summary>
        /// Создание фильтра с по для даты и времени
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="topFilter"></param>
        /// <param name="propName"></param>
        /// <param name="valFrom"></param>
        /// <param name="valTo"></param>
        /// <param name="titleFrom"></param>
        /// <param name="titleTo"></param>
        public static void CreateFilterFromToDateTime(Dictionary<string, FilterItemModel> filter, ICollection<RenderAttribute> topFilter, string propName, string valFrom = null, string valTo = null, string titleFrom = "C", string titleTo = "По")
        {
            var dateTimeFrom = new RenderAttribute
            {
                Control = RenderControl.DateTime,
                DisplayName = titleFrom,
                PropertyName = propName,
                FilterName = "DateTimeFrom"
            };

            var dateTimeTo = new RenderAttribute
            {
                Control = RenderControl.DateTime,
                DisplayName = titleTo,
                PropertyName = propName,
                FilterName = "DateTimeTo"
            };

            CreateFilter(topFilter, filter, dateTimeFrom, valFrom, FilterTypes.GreaterOrEquals);
            CreateFilter(topFilter, filter, dateTimeTo, valTo, FilterTypes.LessOrEquals);
        }

        /// <summary>
        /// Создание фильтра с по для даты и времени по имени свойства
        /// </summary>
        public static void CreatePropertyFilterFromToDateTime(Dictionary<string, FilterItemModel> filter, ICollection<RenderAttribute> topFilter, string propName, string valFrom = null, string valTo = null, string titleFrom = "C", string titleTo = "По")
        {
            var dateTimeFrom = new RenderAttribute
            {
                Control = RenderControl.DateTime,
                DisplayName = titleFrom,
                PropertyName = propName,
                FilterName = $"{propName}From"
            };

            var dateTimeTo = new RenderAttribute
            {
                Control = RenderControl.DateTime,
                DisplayName = titleTo,
                PropertyName = propName,
                FilterName = $"{propName}To"
            };

            CreateFilter(topFilter, filter, dateTimeFrom, valFrom, FilterTypes.GreaterOrEquals);
            CreateFilter(topFilter, filter, dateTimeTo, valTo, FilterTypes.LessOrEquals);
        }

        /// <summary>
        /// Операции
        /// </summary>
        public ICollection<Command> Actions { get; set; }

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public List<string> BreadCrumbs { get; set; }
    }
}
