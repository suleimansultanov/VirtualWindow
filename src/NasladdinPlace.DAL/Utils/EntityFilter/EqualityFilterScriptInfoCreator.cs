using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils.EntityFilter.Contracts;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public class EqualityFilterScriptInfoCreator : IFilterScriptInfoCreator
    {
        private readonly string _entityProperty;
        private readonly FilterItemModel _itemModel;
        private readonly ICastTypeFactory _castTypeFactory;
        private readonly Dictionary<Type, Func<string, object>> _customConverters;

        public EqualityFilterScriptInfoCreator(string entityProperty, FilterItemModel itemModel, ICastTypeFactory castTypeFactory)
        {
            _entityProperty = entityProperty;
            _itemModel = itemModel;
            _castTypeFactory = castTypeFactory;
            _customConverters = new Dictionary<Type, Func<string, object>>
            {
                {typeof(DateTime), ConvertStringToUtcDateTime}
            };
        }

        public FilterScriptInfo Create(int index)
        {
            var script = $"{_entityProperty} {_itemModel.FilterType.GetSign()} @{index}";
            var castType = _castTypeFactory.Create();
            object filterParameter = null;

            if (castType == null)
            {
                filterParameter = _itemModel.Value;
            }
            else
            {
                if (_customConverters.ContainsKey(castType))
                {
                    filterParameter = _customConverters[castType](_itemModel.Value);
                }

                if (filterParameter == null)
                {
                    var converter = TypeDescriptor.GetConverter(castType);
                    filterParameter = converter.ConvertFrom(_itemModel.Value);
                }
            }

            return new FilterScriptInfo(script, filterParameter);
        }

        private object ConvertStringToUtcDateTime(string dateTimeString)
        {
            var convertedDateTime = ConvertStringToDateTime(dateTimeString);
            if (convertedDateTime == null)
                return null;

            var moscowDateTime = (DateTime)convertedDateTime;
            var utcDataTime = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(moscowDateTime);

            return utcDataTime;
        }

        private object ConvertStringToDateTime(string dateTimeString)
        {
            var dateTimeFormats = new[]
            {
                "dd.MM.yyyy hh:mm",
                "dd.MM.yyyy hh:mm:ss",
                "dd.MM.yyyy",
                "yyyy-MM-dd",
                RenderExt.DynamicFilterDateFormat,
                RenderExt.DynamicFilterShortDateFormat
            };

            if (DateTime.TryParse(dateTimeString, out var result))
                return result;
            if (DateTime.TryParseExact(dateTimeString, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return result;

            return null;
        }
    }
}