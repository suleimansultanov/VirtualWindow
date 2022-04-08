using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.HardToDetectLabels
{
    public interface IHardToDetectLabelsPrinter
    {
        Task<string> PrintAsync(int posId, IEnumerable<string> labels);
    }
}