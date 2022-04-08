using NasladdinPlace.UI.ViewModels.GoodCategories;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Services.GoodCategories.Contracts
{
    public interface IGoodCategoryService
    {
        Task<Result> AddGoodCategoryAsync(GoodCategoryViewModel viewModel, int userId);
        Task<Result> EditGoodCategoryAsync(GoodCategoryViewModel viewModel, int userId);
        ValueResult<string> GetBaseApiUrl();
        ValueResult<string> GetDefaultImagePath();
    }
}
