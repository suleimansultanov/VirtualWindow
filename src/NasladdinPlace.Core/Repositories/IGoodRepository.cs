using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Repositories
{
    public interface IGoodRepository : IRepository<Good>
    {
        Task<Good> GetAsync(int id);
        Task<Good> GetIncludingImagesAsync(int id);
        Task<List<Good>> GetAllAsync();
        Task<List<Good>> GetAllPublishedAsync();
        void Remove(Good good);
        IQueryable<Good> GetAllIncludingCategoryAndMaker();

        Task<List<Good>> GetGoodsInCategoryAndInPreparingToPublishOrPublishedAsync(int categoryId, byte pageNumber,
            int pageSize);
        Task<Good> GetByIdIncludingImagesCategoryAndLabeledGoodsAsync(int id);
    }
}
