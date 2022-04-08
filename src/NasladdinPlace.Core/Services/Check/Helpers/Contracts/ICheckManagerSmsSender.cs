using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Helpers.Contracts
{
    public interface ICheckManagerSmsSender
    {
        Task SendSmsAsync(CheckEditingInfo checkEditingInfo);
    }
}
