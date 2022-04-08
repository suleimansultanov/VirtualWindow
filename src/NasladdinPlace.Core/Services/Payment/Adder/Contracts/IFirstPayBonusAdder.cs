using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Payment.Adder.Contracts
{
    public interface IFirstPayBonusAdder
    {
        Task CheckAndAddAvailableUserBonusPointsAsync(int userId);
    }
}