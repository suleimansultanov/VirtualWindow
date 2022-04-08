using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Extensions;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Converter;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Mapper;

namespace NasladdinPlace.Core.Services.OverdueGoods.Makers
{
    public class OverdueGoodsInfoMaker : IOverdueGoodsInfoMaker
    {
        private readonly IGoodInstanceMapper _goodInstanceMapper;
        private readonly IGoodInstancesByPosGrouper _goodInstancesByPosGrouper;

        public OverdueGoodsInfoMaker(
            IGoodInstanceMapper goodInstanceMapper,
            IGoodInstancesByPosGrouper goodInstancesByPosGrouper)
        {
            _goodInstanceMapper = goodInstanceMapper;
            _goodInstancesByPosGrouper = goodInstancesByPosGrouper;
        }
        
        public Dictionary<OverdueType, IEnumerable<PosGoodInstances>> Make(ICollection<LabeledGood> pointsOfSaleContent)
        {
            var overdueInstances = new Dictionary<OverdueType, IEnumerable<PosGoodInstances>>();

            foreach (var type in Enum.GetValues(typeof(OverdueType)))
            {
                var posGoodInstancesByType = GetLabeledGoodsGroppedByPos((OverdueType) type, pointsOfSaleContent).ToImmutableList();
                overdueInstances.Add((OverdueType) type, posGoodInstancesByType);
            }

            return overdueInstances;
        }
        private IEnumerable<PosGoodInstances> GetLabeledGoodsGroppedByPos(OverdueType overdueType, IEnumerable<LabeledGood> pointsOfSaleContent)
        {
            var utcDateTimeRange = overdueType.GetTimeSpanRange().CreateDateTimeRange(DateTime.UtcNow);
            
            var posGoodInstances = pointsOfSaleContent.Where(lg =>
                    lg.ExpirationDate > utcDateTimeRange.Start &&
                    lg.ExpirationDate < utcDateTimeRange.End)
                .Select(ToGoodInstance);

            return GroupByPos(posGoodInstances);
        }

        private GoodInstance ToGoodInstance(LabeledGood labeledGood)
        {
            return _goodInstanceMapper.Transform(labeledGood);
        }

        private IEnumerable<PosGoodInstances> GroupByPos(IEnumerable<GoodInstance> goodInstances)
        {
            return _goodInstancesByPosGrouper.Group(goodInstances);
        }
    }
}