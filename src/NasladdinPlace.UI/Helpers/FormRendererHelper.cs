using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace NasladdinPlace.UI.Helpers
{
    /// <summary>
    /// Класс для получения информации по рендерингу
    /// </summary>
    public class FormRendererHelperBase
    {
        private readonly RenderAttributeFactory _renderAttributeFactory;

        public FormRendererHelperBase()
        {
            _renderAttributeFactory = new RenderAttributeFactory();
        }

        /// <summary>
        /// Получить информацию по рендерингу для всех свойств которые отображаются
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onlyRead"></param>
        /// <returns></returns>
        protected List<FormRendererItemInfo> GetRendererFieldsBase(Type type, bool onlyRead = false)
        {
            var list = GetFieldsBase(type).GetRendererFields().ToList();

            if (!onlyRead) return list;

            foreach (var fInfo in list)
            {
                fInfo.RenderInfo.ReadOnly = onlyRead;
            }
            return list;
        }


        /// <summary>
        /// Получить информацию по рендерингу для всех свойств
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IEnumerable<FormRendererItemInfo> GetFieldsBase(Type type)
        {
            var defRenderParams = type.GetCustomAttribute<RenderAttribute>();

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Select(propertyInfo => GetFieldBase(propertyInfo, defRenderParams))
                 .Where(item => !item.RenderInfo.Ignore);
        }

        /// <summary>
        /// Получить информацию по рендерингу для свойства
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="defRenderParams">Параметры рендеринга по умолчанию</param>
        /// <returns></returns>
        protected FormRendererItemInfo GetFieldBase(PropertyInfo propertyInfo, RenderAttribute defRenderParams = null)
        {
            var renderAttribute = GetRenderInfo(propertyInfo, defRenderParams);
            return new FormRendererItemInfo { Info = propertyInfo, RenderInfo = renderAttribute };
        }

        /// <summary>
        /// Получение информацию по рендерингу для свойства
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="defRenderParams">Параметры рендеринга по умолчанию</param>
        /// <returns></returns>
        private RenderAttribute GetRenderInfo(PropertyInfo propertyInfo, RenderAttribute defRenderParams)
        {
            var renderAttribute = propertyInfo.GetCustomAttribute<RenderAttribute>() ?? _renderAttributeFactory.Create(propertyInfo, defRenderParams);

            renderAttribute.Ignore = propertyInfo.GetCustomAttribute<IgnoreDataMemberAttribute>() != null;

            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            renderAttribute.DisplayName = displayAttr?.Name ?? (renderAttribute.DisplayName ?? propertyInfo.Name);

            var requiredAttr = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            renderAttribute.Required = requiredAttr != null ||
                                       (Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null &&
                                        propertyInfo.PropertyType != typeof(string));

            renderAttribute.PropertyName = propertyInfo.Name;

            var stringLengthAttr = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            renderAttribute.MaxLength = stringLengthAttr?.MaximumLength ?? 0;

            renderAttribute.ComboSourceFullName = renderAttribute.ComboSource?.FullName;

            return renderAttribute;
        }

        public static string GetBindPropertyOption(Type type)
        {
            return $"$root.References['{type.FullName}'].Data";
        }
    }

    /// <summary>
    /// Класс для получения информации по рендерингу
    /// </summary>
    public class FormRendererHelper : FormRendererHelperBase
    {
        public FormRendererHelper() : base()
        {
        }

        /// <summary>
        /// Получить информацию по рендерингу для всех свойств которые отображаются
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onlyRead"></param>
        /// <returns></returns>
        public List<FormRendererItemInfo> GetRendererFields(Type type, bool onlyRead = false)
        {
            return GetRendererFieldsBase(type, onlyRead);
        }

        /// <summary>
        /// Получить информацию по рендерингу для всех свойств
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<FormRendererItemInfo> GetFields(Type type)
        {
            return GetFieldsBase(type);
        }

        /// <summary>
        /// Получить информацию по рендерингу для свойства
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="defRenderParams">Параметры рендеринга по умолчанию</param>
        /// <returns></returns>
        public FormRendererItemInfo GetField(PropertyInfo propertyInfo, RenderAttribute defRenderParams = null)
        {
            return GetFieldBase(propertyInfo, defRenderParams);
        }
    }

    /// <summary>
    /// Класс для получения информации по рендерингу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FormRendererHelper<T> : FormRendererHelperBase
    {
        private readonly Dictionary<string, FormRendererItemInfo> _dictionaryFields;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FormRendererHelper() : base()
        {
            _dictionaryFields = GetFieldsBase(typeof(T)).ToDictionary(x => x.Info.Name, x => x);
        }

        /// <summary>
        /// Получить информацию по рендерингу
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RenderAttribute Get<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var member = (MemberExpression)expression.Body;
            var name = member.Member.Name;

            if (!_dictionaryFields.ContainsKey(name))
                throw new ArgumentException("Не удалось найти поле");

            return _dictionaryFields[name].RenderInfo;
        }
    }

    public static class FormRendererExt
    {
        public static IEnumerable<FormRendererItemInfo> GetRendererFields(this IEnumerable<FormRendererItemInfo> collection)
        {
            return collection?.Where(x => x.RenderInfo.DisplayType != DisplayType.Hide).ToList();
        }

        public static IEnumerable<FormRendererItemInfo> GetFilterFields(this IEnumerable<FormRendererItemInfo> collection)
        {
            return collection?.Where(x => (x.RenderInfo.DisplayType != DisplayType.Hide && x.RenderInfo.FilterState != FilterState.Disable && x.RenderInfo.FilterState != FilterState.Ignore) || x.RenderInfo.FilterState == FilterState.Enable || x.RenderInfo.FilterState == FilterState.EnableNotNull).ToList();
        }

        public static Dictionary<string, FilterItemModel> GetFilterItems(this IEnumerable<FormRendererItemInfo> collection)
        {
            return collection.Where(f => f.RenderInfo.FilterState != FilterState.Ignore).ToDictionary(x => x.Info.Name, x => x.GetFilter());
        }

        public static FilterItemModel GetFilter(this FormRendererItemInfo x)
        {
            return new FilterItemModel
            {
                PropertyName = x.Info.Name,
                Sort = x.RenderInfo.GetSortParameter(),
                FilterType = GetFilterType(x),
                IsNotCleanValue = x.RenderInfo.IsNotCleanValue,
                FilterName = x.RenderInfo.FilterName
            };
        }

        private static FilterTypes GetFilterType(FormRendererItemInfo x)
        {
            switch (x.RenderInfo.Control)
            {
                case RenderControl.Date:
                case RenderControl.Decimal:
                case RenderControl.Integer:
                case RenderControl.YesNo:
                case RenderControl.YesNoEmpty:
                    return FilterTypes.Equals;
                case RenderControl.Input:
                case RenderControl.TextArea:
                case RenderControl.TextReferences:
                    return FilterTypes.Contains;
            }
            return FilterTypes.Equals;
        }

        public static FormRendererItemInfo Get(this List<FormRendererItemInfo> items, string name)
        {
            return items.FirstOrDefault(x => x.Info.Name == name);
        }

        public static void SetValueFilter(this Dictionary<string, FilterItemModel> filters, string filterName, int value)
        {
            SetValueFilter(filters, filterName, value.ToString());
        }
        public static void SetValueFilter(this Dictionary<string, FilterItemModel> filters, string filterName, DateTime value)
        {
            SetValueFilter(filters, filterName, value.ToDynamicFilterDateFormat());
        }
        public static void SetValueFilter(this Dictionary<string, FilterItemModel> filters, string filterName, string value)
        {
            ChangeFilter(filters, filterName, f =>
            {
                f.Value = value;
            });
        }

        public static void SetValueFilter(this Dictionary<string, FilterItemModel> filters, IEnumerable<FormRendererItemInfo> formRendererInfos, Func<FormRendererItemInfo, bool> predicate, string value)
        {
            var item = formRendererInfos.FirstOrDefault(predicate);
            if (item != null)
            {
                SetValueFilter(filters, item.RenderInfo.PropertyName, value);
            }
        }

        public static void SetFilterType(this Dictionary<string, FilterItemModel> filters, string filterName, FilterTypes filterType)
        {
            ChangeFilter(filters, filterName, f =>
            {
                f.FilterType = filterType;
            });
        }

        public static void SetCastType(this Dictionary<string, FilterItemModel> filters, string filterName, CastTypes castType)
        {
            ChangeFilter(filters, filterName, f =>
            {
                f.ForceCastType = castType;
            });
        }

        public static void SetSort(this Dictionary<string, FilterItemModel> filters, string propertyName, SortTypes sortType)
        {
            ChangeFilter(filters, propertyName, f =>
            {
                f.SortType = sortType;
            });
        }

        public static void SetSort(this Dictionary<string, FilterItemModel> filters, string propertyName, SortTypes sortType, int sortOrder)
        {
            ChangeFilter(filters, propertyName, f =>
            {
                f.SortType = sortType;
                f.SortOrder = sortOrder;
            });
        }

        private static void ChangeFilter(IReadOnlyDictionary<string, FilterItemModel> filters, string filterName, Action<FilterItemModel> action)
        {
            FilterItemModel filter;
            if (filters.TryGetValue(filterName, out filter))
            {
                if (filter == null) throw new KeyNotFoundException($"Фильтр '{filterName}' не найден");
                action(filter);
            }
        }

        public static void CreateAdditionFilter(this Dictionary<string, FilterItemModel> filters, CastTypes forceCastType, string filterName, FilterTypes filterType = FilterTypes.Equals, string value = null)
        {
            CreateAdditionFilter(filters, forceCastType, filterName, filterName, filterType, value);
        }

        public static void CreateAdditionFilter(this Dictionary<string, FilterItemModel> filters,
            CastTypes forceCastType, string filterName, string filter, FilterTypes filterType,
            int value)
        {
            CreateAdditionFilter(filters, forceCastType, filterName, filter, filterType, value.ToString());
        }

        public static void CreateAdditionFilter(this Dictionary<string, FilterItemModel> filters, CastTypes forceCastType, string filterName, string filter, FilterTypes filterType = FilterTypes.Equals, string value = null)
        {
            filters.Add(filterName, new FilterItemModel
            {
                PropertyName = filter,
                FilterType = filterType,
                Value = value,
                ForceCastType = forceCastType,
                FilterName = filterName
            });
        }
    }
}
