using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.DAL.Repositories
{
    public class GoodRepository : Repository<Good>, IGoodRepository
    {
        public GoodRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Good> GetAsync(int id)
        {
            return GetAll()
                .Include(g => g.ProteinsFatsCarbohydratesCalories)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public Task<Good> GetIncludingImagesAsync(int id)
        {
            return GetAllIncludingMakerImagesCategoryAndPfcc()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public Task<List<Good>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<Good>> GetAllPublishedAsync()
        {
            return GetAll()
                .Where(g => g.PublishingStatus == GoodPublishingStatus.Published)
                .ToListAsync();
        }

        public void Remove(Good good)
        {
            if(good.ProteinsFatsCarbohydratesCalories != null)
                Context.ProteinsFatsCarbohydratesCalories.Remove(good.ProteinsFatsCarbohydratesCalories);

            Context.GoodImages.RemoveRange(good.GoodImages);

            Context.Remove(good);
        }

        public IQueryable<Good> GetAllIncludingCategoryAndMaker()
        {
            return GetAll()
                .Include(g => g.GoodCategory)
                .Include(g => g.Maker);
        }

        public Task<List<Good>> GetGoodsInCategoryAndInPreparingToPublishOrPublishedAsync(int categoryId, byte pageNumber, int pageSize)
        {
            return GetAllIncludingMakerImagesCategoryAndPfcc()
                .Where(g => g.PublishingStatus != GoodPublishingStatus.NotPublished &&
                            g.GoodCategoryId == categoryId)
                .OrderBy(g => g.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<Good> GetByIdIncludingImagesCategoryAndLabeledGoodsAsync(int id)
        {
            return GetAllIncludingMakerImagesCategoryAndPfcc()
                .Include(g => g.LabeledGoods)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        private IQueryable<Good> GetAllIncludingMakerImagesCategoryAndPfcc()
        {
            return GetAll()
                .Include(g => g.GoodCategory)
                .Include(g => g.GoodImages)
                .Include(g => g.Maker)
                .Include(g => g.ProteinsFatsCarbohydratesCalories);
        }
    }
}
