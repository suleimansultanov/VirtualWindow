using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPromotionSettingRepository
    {
        Task<PromotionSetting> GetByPromotionTypeAsync(PromotionType promotionType);
    }
}
