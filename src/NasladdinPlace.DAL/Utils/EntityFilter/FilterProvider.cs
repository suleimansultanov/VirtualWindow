using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter.Contracts;
using NasladdinPlace.DAL.Utils.Models;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    /// <summary>
    /// Провайдер который формирует модель для динамической фильтрации
    /// </summary>
    public class FilterProvider
    {
        private const string ConstOr = " OR ";
        private const string ConstAnd = " AND ";
        public const string ConstAsc = "ASC";
        public const string ConstDesc = "DESC";

        private readonly Type _type;
        private int _index;

        public List<object> Params { get; set; } = new List<object>();
        public string Predicate { get; set; }

        private readonly Dictionary<Type, Func<BaseFilterItemModel, string>> _filterItemProcessor;

        private readonly IFilterScriptInfoCreatorFactory _filterScriptInfoCreatorFactory;

        public FilterProvider(Type type)
        {
            _type = type;
            _filterScriptInfoCreatorFactory = new FilterScriptInfoCreatorFactory();
            _filterItemProcessor = new Dictionary<Type, Func<BaseFilterItemModel, string>>
            {
                {typeof(FilterAnd), FilterAndProcess},
                {typeof(FilterOr), FilterOrProcess},
                {typeof(FilterItemModel), FilterItemModelProcess},
            };
        }

        public DynamicFilter GetFilter(List<BaseFilterItemModel> list)
        {
            var predicate = FilterDelimiter(list, ConstAnd);

            var result = new DynamicFilter
            {
                Predicate = predicate,
                Params = Params.ToArray(),
                Sort = GetSort(list)
            };

            return result;
        }

        public string GetSort(List<BaseFilterItemModel> list)
        {
            var sort = GetSortList(list).OrderBy(x => x.SortOrder).Select(x =>
            {
                var strings = x.Sort.Split(',');
                var sortType = x.SortType == SortTypes.Asc ? ConstAsc : ConstDesc;

                return strings.Length > 1
                    ? string.Join(", ",
                        strings.Select(i => $"{i} {sortType}"))
                    : $"{x.Sort} {sortType}";
            });
            return string.Join(", ", sort);
        }

        private string CreateFilterString(BaseFilterItemModel filterItemModel)
        {
            var type = filterItemModel.GetType();

            Func<BaseFilterItemModel, string> processor;
            if (_filterItemProcessor.TryGetValue(type, out processor))
            {
                return processor.Invoke(filterItemModel);
            }

            throw new NotImplementedException("Не удалось сфоримровать фильтр по типу " + type);
        }

        private string FilterAndProcess(BaseFilterItemModel baseFilterItemModel)
        {
            return FilterDelimiter(baseFilterItemModel.Items, ConstAnd);
        }

        private string FilterOrProcess(BaseFilterItemModel baseFilterItemModel)
        {
            return FilterDelimiter(baseFilterItemModel.Items, ConstOr);
        }

        private string FilterDelimiter(IEnumerable<BaseFilterItemModel> items, string delimiter)
        {
            var list = items.Select(CreateFilterString).Where(x => !string.IsNullOrEmpty(x));
            var filterAndProcess = string.Join(delimiter, list);
            if (!string.IsNullOrEmpty(filterAndProcess))
            {
                filterAndProcess = $"({filterAndProcess})";
            }

            return filterAndProcess;
        }

        private string FilterItemModelProcess(BaseFilterItemModel baseFilterItemModel)
        {
            var itemModel = (FilterItemModel)baseFilterItemModel;

            var entityFilterContext = new EntityFilterContext(itemModel, _type);

            if (entityFilterContext.IsPropertyAndItsValueExist)
            {
                var filterScriptInfoCreator = _filterScriptInfoCreatorFactory.Create(entityFilterContext);
                var filterScriptInfo = filterScriptInfoCreator.Create(_index);
                if (filterScriptInfo.FilterParameter != null)
                {
                    Params.Add(filterScriptInfo.FilterParameter);
                    _index++;
                    return filterScriptInfo.Script;
                }
            }

            return null;
        }

        public static IEnumerable<FilterItemModel> GetSortList(IEnumerable<BaseFilterItemModel> items)
        {
            foreach (var item in items)
            {
                if (item is FilterItemModel cast && cast.SortType != SortTypes.None)
                    yield return cast;

                if (item?.Items == null || !item.Items.Any())
                    continue;

                foreach (var child in GetSortList(item.Items))
                {
                    yield return child;
                }
            }
        }

        public string GetFirstSortProperty()
        {
            var props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.GetGetMethod().IsVirtual).ToList();
            foreach (var propertyInfo in props)
            {
                var validProperty = PropertyInfoGenerator.GetValidProperty(propertyInfo.Name, _type);
                if (validProperty != null)
                    return propertyInfo.Name;
            }

            throw new InvalidOperationException($"Не удалось найти свойство для сортировки");
        }
    }
}
