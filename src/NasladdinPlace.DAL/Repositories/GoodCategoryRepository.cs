using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Models.LabeledGoods;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class GoodCategoryRepository : Repository<GoodCategory>, IGoodCategoryRepository
    {
        private readonly IApplicationDbContext _context;

        public GoodCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<GoodCategory> GetAsync(int id)
        {
            return GetAll()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<List<GoodCategory>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<GoodCategory>> GetByNameAsync(string name)
        {
            return GetAll()
                .Where(c => c.Name == name)
                .ToListAsync();
        }

        public Task<Dictionary<GoodCategory, List<LabeledGood>>> GetGoodCategoriesDictionaryWithLabeledGoodsInPosAsync(
            int posId, byte pageNumber, int pageSize, int categoriesSize)
        {
            var labeledGoods = _context.LabeledGoods
                    .Include(gc => gc.Good)
                    .ThenInclude(g => g.GoodCategory)
                    .Include(gc => gc.Good)
                    .ThenInclude(g => g.Maker)
                    .Include(gc => gc.Good)
                    .ThenInclude(g => g.ProteinsFatsCarbohydratesCalories)
                    .Include(gc => gc.Good)
                    .ThenInclude(g => g.GoodImages)
                    .Include(lg => lg.Currency)
                    .Where(lg => !lg.IsDisabled && lg.PosOperationId == null && lg.PosId != null && lg.PosId == posId);

            return MakeDictionary(labeledGoods, categoriesSize, pageNumber, pageSize);
        }

        public Task<Dictionary<GoodCategory, List<Good>>> GetGoodCategoriesDictionaryWithLabeledGoodsAsync(
            byte pageNumber, int pageSize, int categoriesSize)
        {
            return _context.Goods
                .Include(g => g.GoodCategory)
                .Include(g => g.Maker)
                .Include(g => g.ProteinsFatsCarbohydratesCalories)
                .Include(g => g.GoodImages)
                .Where(g => g.PublishingStatus != GoodPublishingStatus.NotPublished)
                .GroupBy(g => g.GoodCategory, (category, goods) => new
                {
                    Key = category,
                    Goods = goods
                })
                .OrderBy(gc => gc.Key.Name)
                .Skip(categoriesSize * (pageNumber - 1))
                .Take(categoriesSize)
                .ToDictionaryAsync(gc => gc.Key,
                    selector => selector.Goods
                        .OrderBy(g => g.Name)
                        .Take(pageSize)
                        .ToList());
        }

        private Task<Dictionary<GoodCategory, List<LabeledGood>>> MakeDictionary(IQueryable<LabeledGood> queryableLabeledGoods,
            int categoriesSize, int pageNumber, int pageSize)
        {
            return queryableLabeledGoods
                .GroupBy(lg => lg.Good.GoodCategory, (category, labeledGoods) => new
                {
                    Key = category,
                    LabeledGoods = labeledGoods
                })
                .OrderBy(gc => gc.Key.Name)
                .Skip(categoriesSize * (pageNumber - 1))
                .Take(categoriesSize)
                .ToDictionaryAsync(gc => gc.Key,
                    selector => selector.LabeledGoods.Distinct(new LabeledGoodComparer())
                        .OrderBy(lg => lg.Good.Name)
                        .Take(pageSize)
                        .ToList());
        }
    }
}