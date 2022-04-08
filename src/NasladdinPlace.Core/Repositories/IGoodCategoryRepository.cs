using NasladdinPlace.Core.Models.Goods;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IGoodCategoryRepository : IRepository<GoodCategory>
    {
        Task<GoodCategory> GetAsync(int id);
        Task<List<GoodCategory>> GetAllAsync();
        Task<List<GoodCategory>> GetByNameAsync(string name);
        Task<Dictionary<GoodCategory, List<LabeledGood>>> GetGoodCategoriesDictionaryWithLabeledGoodsInPosAsync(int posId, byte pageNumber, int pageSize, int categoriesSize);
        Task<Dictionary<GoodCategory, List<Good>>> GetGoodCategoriesDictionaryWithLabeledGoodsAsync(byte pageNumber, int pageSize, int categoriesSize);
    }
}