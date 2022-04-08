using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.LabelsDetectionStatistics.Models;

namespace NasladdinPlace.Core.Services.LabelsDetectionStatistics
{
    public interface ILabelsDetectionStatisticsMaker
    {
        Task<IEnumerable<LabelDetectionInfo>> Make(IEnumerable<string> labelsWithDetectionCounter, char delimeter);
    }
}