using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator
{
    public interface IPosLabeledGoodsFromLabelsCreator
    {
        Task<ICollection<LabeledGood>> CreateAsync(IUnitOfWork unitOfWork, PosContent posContent);
    }
}