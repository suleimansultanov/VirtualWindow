using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.LabeledGoodsMerger.Contracts
{
    public interface ILabeledGoodsMerger
    {
        Task<bool> MergeAsync(
            IUnitOfWork unitOfWork,
            int posId, 
            ICollection<string> putLabeledGoodsLabels,
            ICollection<string> takenLabeledGoodsLabels);
    }
}
