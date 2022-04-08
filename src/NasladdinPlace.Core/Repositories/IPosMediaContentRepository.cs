using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    public interface IPosMediaContentRepository
    {
        Task<List<PosMediaContent>> GetByPosAsync(int posId);

        Task<PosMediaContent> GetByPosIdAndScreenImageTypeAsync(int posId, PosScreenType screenType);

        Task<PosMediaContent> GetByPosIdMediaContentIdAsync(int posId, int mediaContentId);

        IQueryable<PosMediaContent> GetAll();

        void Add(PosMediaContent posMediaContent);

        void Remove(PosMediaContent posMediaContent);
    }
}
