using NasladdinPlace.Core.Models.Goods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IGoodImageRepository
    {
        Task<List<GoodImage>> GetByGoodAsync(int goodId);
        Task<GoodImage> GetByGoodIdAsync(int goodId);
        Task<GoodImage> GetAsync(int goodImageId);
        void Add(GoodImage goodImage);
        void Remove(GoodImage goodImage);
        void Update(GoodImage goodImage);
    }
}
