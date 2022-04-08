using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.UI.ViewModels.Discounts;

namespace NasladdinPlace.UI.Managers.Discounts
{
    public interface IDiscountsManager
    {
        Task AddAsync(DiscountInfoViewModel discountInfoViewModel);

        Task UpdateAsync(DiscountInfoViewModel discountInfoViewModel, ApplicationUser user);
            
        bool Validate(DiscountInfoViewModel discountInfo, out string message);

        DiscountInfoViewModel GetDiscountViewModel(Discount discount);
    }
}
