using NasladdinPlace.UI.ViewModels.Goods;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Services.Goods.Contracts
{
    public interface IGoodService
    {
        Task<ValueResult<string>> AddGoodAsync(GoodsFormViewModel viewModel, int userId);
        Task<Result> EditGoodAsync(GoodsFormViewModel viewModel, int userId);
        Task<Result> DeleteGoodAsync(int id, int userId);
        ValueResult<string> GetBaseApiUrl();
    }
}
