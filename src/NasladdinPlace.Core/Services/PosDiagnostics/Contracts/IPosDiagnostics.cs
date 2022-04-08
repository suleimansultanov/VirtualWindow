using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Contracts
{
    public interface IPosDiagnostics
    {
        Task<Result> PerformAsync();
    }
}