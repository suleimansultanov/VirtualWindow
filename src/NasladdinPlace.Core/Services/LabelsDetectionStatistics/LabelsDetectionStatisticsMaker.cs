using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.LabelsDetectionStatistics.Models;

namespace NasladdinPlace.Core.Services.LabelsDetectionStatistics
{
    public class LabelsDetectionStatisticsMaker : ILabelsDetectionStatisticsMaker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public LabelsDetectionStatisticsMaker(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task<IEnumerable<LabelDetectionInfo>> Make(
            IEnumerable<string> labelsWithDetectionCounter, char delimeter)
        {
            var detectionInfoByLabelMap = labelsWithDetectionCounter.Select(lwdc =>
            {
                var labelWithReadCount = lwdc.Split(delimeter);
                return new LabelDetectionInfo(labelWithReadCount[0], int.Parse(labelWithReadCount[1]));
            }).ToImmutableDictionary(di => di.Label);


            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var labeledGoods =
                    await unitOfWork.LabeledGoods.GetEnabledByLabelsAsync(detectionInfoByLabelMap.Keys);
                foreach (var labeledGood in labeledGoods)
                {
                    var detectionInfo = detectionInfoByLabelMap[labeledGood.Label];
                    detectionInfo.BindToLabeledGood(labeledGood);
                }
            }
            
            return detectionInfoByLabelMap.Values.OrderBy(ldi => ldi.ReadCount).ToImmutableList();
        }
    }
}