using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models.Mapper
{
    public class MoscowGoodInstanceMapper : IGoodInstanceMapper
    {
        public GoodInstance Transform(LabeledGood labeledGood)
        {
            var good = labeledGood.Good ?? Good.Unknown;
          
            var priceWithCurrency = $"{labeledGood.Price:N2} {labeledGood.Currency.Name}";

            var expirationDate = ConvertToMoscowDateTimeString(labeledGood.ExpirationDate ?? DateTime.MaxValue);
            
            return new GoodInstance(
                name: good.Name,
                price: priceWithCurrency,
                label: labeledGood.Label ?? string.Empty,
                expirationDate: expirationDate,
                posId: labeledGood.Pos.Id,
                posName: labeledGood.Pos.AbbreviatedName);
        }

        public IEnumerable<GoodInstance> TransformCollection(IEnumerable<LabeledGood> labeledGoods)
        {
            return labeledGoods
                .Where(lg => lg != null)
                .Select(Transform)
                .ToImmutableList();
        }
        
        private static string ConvertToMoscowDateTimeString(DateTime utcDateTime)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(utcDateTime);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }
    }
}