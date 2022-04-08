using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Services.Catalog.Models;
using NasladdinPlace.Dtos.Catalog;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Api.Services.Catalog.Contracts
{
    public interface ICatalogService
    {
        Task<ValueResult<CatalogPointOfSale>> GetPointsOfSaleAsync(byte page, int userId);
        Task<ValueResult<List<PosContent>>> GetPosContentAsync(int posId, byte pageNumber);
        Task<ValueResult<List<LabeledGoodWithImageDto>>> GetCategoryItemsAsync(CategoryItemsDto categoryItemsDto);
        Task<ValueResult<List<VirtualPosContent>>> GetVirtualPosContentAsync(byte pageNumber);
        Task<ValueResult<List<GoodWithImageAndNutrients>>> GetVirtualCategoryItemsAsync(CategoryItemsDto categoryItemsDto);
    }
}
