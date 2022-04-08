using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PurchasesResetter
{
    public interface IUserPurchasesResetter
    {
        Task Reset(int userId);
    }
}